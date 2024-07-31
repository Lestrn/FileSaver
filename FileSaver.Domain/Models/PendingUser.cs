namespace FileSaver.Domain.Models
{
    using FileSaver.Domain.Interfaces;

    public class PendingUser : IEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string CorrectCode { get; set; }
    }
}
