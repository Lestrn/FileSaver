namespace FileSaver.Domain.Models.Mapping.Models
{
    using FileSaver.Domain.Enums;

    public class UserModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public UserRoles Role { get; set; }
    }
}
