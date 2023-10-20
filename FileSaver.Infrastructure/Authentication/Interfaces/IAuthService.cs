﻿namespace FileSaver.Infrastructure.Authentication.Interfaces
{
    using FileSaver.Domain.DTOs;
    using Newtonsoft.Json.Linq;

    public interface IAuthService
    {
        public Task<JObject> LogIn(UserLoginDTO user);

        public Task<JObject> Register(UserDTO user);

        public Task<bool> ConfirmCode(string email, string userCode, bool addToDatabase = true);

        public Task<JObject> RecoverAccount(string email);

        public Task<JObject> RecoveryLogIn(string email, string userCode);
    }
}
