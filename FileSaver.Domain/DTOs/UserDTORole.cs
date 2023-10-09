using FileSaver.Domain.Enums;

namespace FileSaver.Domain.DTOs
{
    public class UserDTORole
    {
        public Guid Id { get; set; }
        public UserRoles Role { get; set; }
    }
}
