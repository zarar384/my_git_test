using FileMonitorWebApp.Models;
using System.Text.Json;

namespace FileMonitorWebApp.Services
{
    /// <summary>
    /// Service responsible for loading and saving the snapshot of the directory state to a JSON file.
    /// </summary>
    public class SnapshotService
    {
        private readonly ILogger<SnapshotService> _logger;
        private readonly string _snapshotFilePath;

        public SnapshotService(ILogger<SnapshotService> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _snapshotFilePath = Path.Combine(environment.ContentRootPath, "Data", "snapshot.json");
        }

        /// <summary>
        /// Loads the snapshot from the JSON file. 
        /// If the file does not exist or is empty, returns a new Snapshot instance.
        /// </summary>
        /// <param name="snapshot">The loaded <see cref="Snapshot"/>.</param>
        /// <returns>True if the snapshot was loaded successfully; otherwise, false.</returns>
        public bool TryLoad(out Snapshot snapshot)
        {
            snapshot = new Snapshot();

            if (!File.Exists(_snapshotFilePath))
                return true;

            try
            {
                // Read the snapshot file and deserialize it into a Snapshot object
                var json = File.ReadAllText(_snapshotFilePath);

                if(string.IsNullOrWhiteSpace(json))
                {
                    _logger.LogInformation("Snapshot file is empty: {SnapshotFilePath}", _snapshotFilePath);
                    return true;
                }

                snapshot = JsonSerializer.Deserialize<Snapshot>(json) ?? new Snapshot();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load snapshot from file: {SnapshotFilePath}", _snapshotFilePath);
                return false;
            }
        }

        /// <summary>
        /// Saves the given snapshot to the JSON file. 
        /// If the file does not exist, it will be created.
        /// </summary>
        /// <param name="snapshot">The snapshot to save.</param>
        /// <exception cref="InvalidOperationException">Thrown when the snapshot file cannot be saved.</exception>
        public void Save(Snapshot snapshot)
        {
            try
            {
                var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(_snapshotFilePath) ?? string.Empty);
                File.WriteAllText(_snapshotFilePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save snapshot to file: {SnapshotFilePath}", _snapshotFilePath);
                throw new InvalidOperationException("Failed to save the snapshot file.", ex);
            }
        }
    }
}
