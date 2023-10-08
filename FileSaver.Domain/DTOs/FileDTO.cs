namespace FileSaver.Domain.DTOs
{
    public class FileDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
