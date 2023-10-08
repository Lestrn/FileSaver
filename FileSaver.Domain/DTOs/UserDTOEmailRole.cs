using FileSaverApi.Enums;

namespace FileSaverApi.Models
{
    public class UserDTOEmailRole
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
