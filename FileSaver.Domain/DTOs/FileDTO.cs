namespace FileSaverApi.Models
{
    public class FileDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
