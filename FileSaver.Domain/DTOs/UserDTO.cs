using Microsoft.Extensions.FileProviders;

namespace FileSaverApi.Models
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
