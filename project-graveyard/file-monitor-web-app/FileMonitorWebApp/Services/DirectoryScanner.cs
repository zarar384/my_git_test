using FileMonitorWebApp.Models;

namespace FileMonitorWebApp.Services
{
    public class DirectoryScanner
    {
        private readonly ILogger<DirectoryScanner> _logger;
        private readonly FileHashService _hashService;

        public DirectoryScanner(
            ILogger<DirectoryScanner> logger, 
            FileHashService hashService)
        {
            _logger = logger;
            _hashService = hashService;
        }

        public List<FileMetadata> ScanDirectory(string directoryPath)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException("Directory path is required.", nameof(directoryPath));
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            var files = new List<FileMetadata>();

            try
            {
                var options = new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    ReturnSpecialDirectories = false
                };

                // Process files in the directory and prepare metadata
                foreach (var filePath in Directory.EnumerateFiles(directoryPath, "*", options))
                {
                    try
                    {
                        var metadata = new FileMetadata
                        {
                            Path = filePath,
                            Hash = _hashService.ComputeHash(filePath),
                            Version = 1
                        };

                        files.Add(metadata);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to inspect file: {FilePath}", filePath);
                    }
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
            {
                _logger.LogError(ex, "Error scanning directory: {DirectoryPath}", directoryPath);
                throw;
            }

            return files;
        }
    }
}
