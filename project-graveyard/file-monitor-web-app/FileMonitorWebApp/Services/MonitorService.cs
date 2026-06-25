using FileMonitorWebApp.Models;

namespace FileMonitorWebApp.Services
{
    /// <summary>
    /// Service responsible for analyzing the directory state by comparing the current scan results with the previously saved snapshot.
    /// </summary>
    public class MonitorService(
        ILogger<MonitorService> logger,
        DirectoryScanner scanner,
        SnapshotService snapshotService)
    {
        private readonly ILogger<MonitorService> _logger = logger;
        private readonly DirectoryScanner _scanner = scanner;
        private readonly SnapshotService _snapshotService = snapshotService;

        /// <summary>
        /// Analyzes the specified directory by comparing the current state with the previously saved snapshot. 
        /// It identifies new, modified, and deleted files, and returns a ScanResult containing the findings.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to analyze.</param>
        /// <returns>A <see cref="ScanResult"/> containing the analysis results.</returns>
        public ScanResult Analyze(string directoryPath)
        {
            var result = new ScanResult();

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                result.ErrorMessage = "Directory path is required.";
                return result;
            }

            try
            {
                // Load the previous snapshot
                if (!_snapshotService.TryLoad(out var oldSnapshot))
                {
                    _logger.LogWarning("Directory analysis was aborted because the snapshot could not be loaded.");
                    result.ErrorMessage = "Failed to load the previous snapshot.";
                    return result;
                }

                // Scan the current state of the directory
                var currentFiles = _scanner.ScanDirectory(directoryPath);

                // First time scanning, just save the snapshot without comparisons
                // This will initialize the snapshot for future comparisons
                if (!oldSnapshot.Files.Any())
                {
                    TrySaveSnapshot(currentFiles, result);
                    return result;
                }

                var oldFilesDict = oldSnapshot.Files.ToDictionary(f => f.Path);
                var currentFilesDict = currentFiles.ToDictionary(f => f.Path);

                // Identify new and modified files
                foreach (var current in currentFiles)
                {
                    // Check if the file is new
                    if (!oldFilesDict.TryGetValue(current.Path, out var oldFile))
                    {
                        result.NewFiles.Add(current);
                        continue;
                    }

                    // Check if the file has been modified
                    // Increment version if the hash has changed
                    if (oldFile.Hash != current.Hash)
                    {
                        current.Version = oldFile.Version + 1; 
                        
                        result.ModifiedFiles.Add(current);
                    }
                    // If the file is unchanged, keep the same version
                    else
                    {
                        current.Version = oldFile.Version; 
                    }
                }

                // Check if any files have been deleted
                foreach (var old in oldSnapshot.Files)
                {
                    if (!currentFilesDict.ContainsKey(old.Path))
                    {
                        result.DeletedFiles.Add(old);
                    }
                }

                TrySaveSnapshot(currentFiles, result);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogWarning(ex, "Directory was not found: {DirectoryPath}", directoryPath);
                result.ErrorMessage = ex.Message;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Access denied while scanning directory: {DirectoryPath}", directoryPath);
                result.ErrorMessage = "Access denied while scanning the directory.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error analyzing directory: {DirectoryPath}", directoryPath);
                result.ErrorMessage = "An unexpected error occurred while scanning the directory.";
            }

            return result;
        }

        /// <summary>
        /// Attempts to save the snapshot and records any save failure in the result.
        /// </summary>
        /// <param name="files">The list of files to persist in the snapshot.</param>
        /// <param name="result">The scan result to populate with an error message on failure.</param>
        private void TrySaveSnapshot(List<FileMetadata> files, ScanResult result)
        {
            try
            {
                _snapshotService.Save(new Snapshot { Files = files });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to save snapshot.");
                result.ErrorMessage = ex.Message;
            }
        }
    }
}
