using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FileSaver.Domain.Interfaces;

namespace FileSaver.Domain.Models
{
    [Table("Files")]
    public class File : IEntity
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        [ForeignKey("OriginalOwner")]
        [Required]
        public Guid UserDbModelId { get; set; }
        public List<User>? SharedWith { get; set; }

    }
}
