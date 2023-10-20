namespace FileSaver.API.Controllers
{
    using FileSaver.Domain.DTOs;
    using FileSaver.Infrastructure.Authentication.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogIn(UserLoginDTO user)
        {
            JObject res = await this.authService.LogIn(user);
            return this.Ok(res.ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            JObject res = await this.authService.Register(user);
            return this.Ok(res.ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmCode(string email, string code)
        {
            bool isRegistered = await this.authService.ConfirmCode(email, code);
            if (!isRegistered)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RecoverAccount(string email)
        {
            JObject res = await this.authService.RecoverAccount(email);
            return this.Ok(res.ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmRecoveryCode(string email, string userCode)
        {
            JObject res = await this.authService.RecoveryLogIn(email, userCode);
            return this.Ok(res.ToString());
        }
    }
}
