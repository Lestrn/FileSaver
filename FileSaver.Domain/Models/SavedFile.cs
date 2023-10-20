namespace FileSaver.Domain.Models
{
    using FileSaver.Domain.Interfaces;

    public class SavedFile : IEntity
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
