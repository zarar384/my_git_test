namespace FileMonitorWebApp.Models
{
    public class ScanResult
    {
        public List<FileMetadata> NewFiles { get; set; } = new();
        public List<FileMetadata> ModifiedFiles { get; set; } = new();
        public List<FileMetadata> DeletedFiles { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    }
}
