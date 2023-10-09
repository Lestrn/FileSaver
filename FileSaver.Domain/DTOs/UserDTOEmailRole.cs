using FileSaver.Domain.Enums;

namespace FileSaver.Domain.DTOs
{
    public class UserDTOEmailRole
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }
    }
}
