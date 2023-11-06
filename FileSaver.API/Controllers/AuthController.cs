namespace FileSaver.API.Controllers
{
    using FileSaver.Domain.DTOs;
    using FileSaver.Domain.Models;
    using FileSaver.Infrastructure.Authentication.Interfaces;
    using Microsoft.AspNetCore.Cors;
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
            var res = await this.authService.LogIn(user);
            if (!res.isSuccessful)
            {
                return this.BadRequest(res.Item1.ToString());
            }

            return this.Ok(res.Item1.ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            var res = await this.authService.Register(user);
            if (!res.isSuccessful)
            {
                return this.BadRequest(res.Item1.ToString());
            }

            return this.Ok(res.Item1.ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmCode(UserConfirmCodeDTO userConfirmCodeDTO)
        {
            bool isRegistered = await this.authService.ConfirmCode(userConfirmCodeDTO.Email, userConfirmCodeDTO.Code);
            if (!isRegistered)
            {
                return this.BadRequest();
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RecoverAccount(RecoverUserDTO recoverDTO)
        {
            var res = await this.authService.RecoverAccount(recoverDTO.Email);
            if (!res.isSuccessful)
            {
                return this.BadRequest(res.Item1.ToString());
            }

            return this.Ok(res.Item1.ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmRecoveryCode(UserConfirmCodeDTO userConfirmCodeDTO)
        {
            var res = await this.authService.RecoveryLogIn(userConfirmCodeDTO.Email, userConfirmCodeDTO.Code);
            if (!res.isSuccessful)
            {
                return this.BadRequest(res.Item1.ToString());
            }

            return this.Ok(res.Item1.ToString());
        }
    }
}
