namespace FileSaver.Infrastructure.Authentication.Interfaces
{
    using FileSaver.Domain.DTOs;
    using Newtonsoft.Json.Linq;

    public interface IAuthService
    {
        public Task<(JObject, bool isSuccessful)> LogIn(UserLoginDTO user);

        public Task<(JObject, bool isSuccessful)> Register(UserDTO user);

        public Task<bool> ConfirmCode(string email, string userCode, bool addToDatabase = true);

        public Task<(JObject, bool isSuccessful)> RecoverAccount(string email);

        public Task<(JObject, bool isSuccessful)> RecoveryLogIn(string email, string userCode);
    }
}
