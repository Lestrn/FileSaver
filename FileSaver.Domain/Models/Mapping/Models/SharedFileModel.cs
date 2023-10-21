namespace FileSaver.Domain.Models.Mapping.Models
{
    public class SharedFileModel
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }

        public Guid SharedWithUserId { get; set; }
    }
}
