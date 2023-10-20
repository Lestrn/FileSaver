namespace FileSaver.Domain.Models.Mapping.Models
{
    using FileSaver.Domain.Enums;

    public class UserModelEmailRole
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public UserRoles Role { get; set; }
    }
}
