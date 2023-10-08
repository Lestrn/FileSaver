using FileSaver.Application.Interfaces;
using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FileSaver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFile(Guid userId, IFormFile file)
        {
            return await _userService.UploadFile(userId, file) ? Ok() : BadRequest();
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            FileDbModel file = await _userService.GetFileById(fileId);
            if (file != null)
            {
                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName,
                    Inline = false,
                };

                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
                return File(file.Content, file.ContentType);
            }
            return NotFound();
        }
        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(Guid userId, string role)
        {
            bool roleIsParsed = Enum.TryParse(typeof(UserRoles), role, out object? roleEnum);
            if (!roleIsParsed)
            {
                return BadRequest("incorrect role input");
            }

            bool isUpdated = await _userService.UpdateRole(userId, (UserRoles)roleEnum);
            if (!isUpdated)
            {
                return BadRequest("User with such id wasnt found");
            }
            return Ok();
        }
        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> GetAllUsers()
        {
            List<UserDTOEmailRole> userDTOEmailRole = await _userService.GetAllUsers();
            return new JsonResult(userDTOEmailRole);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> GetAllFilesByUserId(Guid userId)
        {
            List<FileDTO> fileDbModels = await _userService.GetAllFilesByUserId(userId);
            return new JsonResult(fileDbModels);
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteFileById(Guid fileId)
        {
            return await _userService.DeleteFile(fileId) ? Ok() : NotFound();
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UploadAvatar(Guid userId, IFormFile image)
        {
            var uploadResult = await _userService.UploadAvatar(userId, image);
            if (!uploadResult.isUploaded)
            {
                return BadRequest(uploadResult.errorMsg);
            }
            return Ok();
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> ChangePassword(Guid userId, string newPassword)
        {
            var res = await _userService.ChangePassword(userId, newPassword);
            if(!res.isChanged)
            {
                return BadRequest(res.message);
            }
            return Ok();
        }
    }
}
