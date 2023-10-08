using FileSaver.Domain.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Infrastructure.Authentication.Interfaces
{
    public interface IAuthService
    {
        public Task<JObject> LogIn(UserLoginDTO user);
        public Task<JObject> Register(UserDTO user);
        public Task<bool> ConfirmCode(string email, string userCode, bool addToDatabase = true);
        public Task<bool> DeleteAccount(UserDTODelete user);
        public Task<JObject> RecoverAccount(string email);
        public Task<JObject> RecoveryLogIn(string email, string userCode);
    }
}
