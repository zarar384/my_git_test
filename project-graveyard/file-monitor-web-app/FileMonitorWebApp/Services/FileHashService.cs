using System.Security.Cryptography;

namespace FileMonitorWebApp.Services
{
    /// <summary>
    /// Service responsible for computing the hash of a file using SHA256.
    /// </summary>
    public class FileHashService(ILogger<FileHashService> logger)
    {
        private readonly ILogger<FileHashService> _logger = logger;

        /// <summary>
        /// Computes the SHA256 hash of the specified file and returns it as a Base64 string.
        /// </summary>
        /// <param name="filePath">The path of the file to compute the hash for.</param>
        /// <returns>The SHA256 hash of the file as a Base64 string.</returns>
        /// <exception cref="ArgumentException">Thrown when the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
        public string ComputeHash(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}", filePath);
            }

            try
            {
                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);

                // Compute the hash of the file and convert it to a Base64 string for easier storage
                var hashBytes = sha256.ComputeHash(stream);

                return Convert.ToBase64String(hashBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compute hash for file: {FilePath}", filePath);
                throw;
            }
        }
    }
}