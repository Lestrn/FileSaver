namespace FileSaver.Domain.Models.Mapping.Models
{
    public class SavedFileModel
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }
    }
}
