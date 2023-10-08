using FileSaver.Domain.DTOs;
using FileSaver.Infrastructure.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FileSaver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        public AuthController(IAuthService userService)
        {
            _authService = userService;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogIn(UserLoginDTO user)
        {
            JObject res = await _authService.LogIn(user);
            return Ok(res.ToString());
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            JObject res = await _authService.Register(user);
            return Ok(res.ToString());
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmCode(string email, string code)
        {
            bool isRegistered = await _authService.ConfirmCode(email, code);
            if (!isRegistered)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpDelete]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount(UserDTODelete user)
        {
            bool isDeleted = await _authService.DeleteAccount(user);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RecoverAccount(string email)
        {
            JObject res = await _authService.RecoverAccount(email);
            return Ok(res.ToString());
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ConfirmRecoveryCode(string email, string userCode)
        {
            JObject res =  await _authService.RecoveryLogIn(email, userCode);
            return Ok(res.ToString()); 
        }
    }
}
