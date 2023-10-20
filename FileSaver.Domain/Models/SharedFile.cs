namespace FileSaver.Domain.Models
{
    using FileSaver.Domain.Interfaces;

    public class SharedFile : IEntity
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }

        public SavedFile File { get; set; }

        public Guid SharedByUserId { get; set; } 

        public User SharedByUser { get; set; }

        public Guid SharedWithUserId { get; set; } 

        public User SharedWithUser { get; set; }
    }

}
