using FileSaver.Domain.Enums;
using FileSaver.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileSaver.Domain.Models
{
    [Table("PendingUsers")]
    public class PendingUser : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CorrectCode { get; set; }
    }
}
