namespace FileSaver.Domain.DTOs
{
    using FileSaver.Domain.Enums;

    public class UserDTORole
    {
        public Guid Id { get; set; }

        public UserRoles Role { get; set; }
    }
}
