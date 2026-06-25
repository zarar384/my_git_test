namespace FileMonitorWebApp.Models
{
    public class FileMetadata
    {
        public string Path { get; init; }
        public string Hash { get; init; }
        public int Version { get; set; }
    }
}
