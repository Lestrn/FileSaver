using FileSaver.Application.Interfaces;
using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Models;
using FileSaver.Domain.Models.Mapping.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection.Metadata.Ecma335;

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
            SavedFile file = await _userService.GetFileById(fileId);
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
            List<UserModelEmailRole> userDTOEmailRole = await _userService.GetAllUsers();
            return new JsonResult(userDTOEmailRole);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> GetAllFilesByUserId(Guid userId)
        {
            List<SavedFileModel> fileDbModels = await _userService.GetAllFilesByUserId(userId);
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
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteAccount(UserDTODelete user)
        {
            bool isDeleted = await _userService.DeleteAccount(user);
            if (!isDeleted)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ShareFile(UserFileShareDTO share)
        {
            bool isShared = await _userService.ShareFile(share);
            if (!isShared)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> SendFriendRequest(Guid senderid, string receiverUsername)
        {
            var res = await _userService.SendFriendRequest(senderid, receiverUsername);
            if (!res.isSent)
            {
                return BadRequest(res.errorMsg);
            }
            return Ok();
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> AcceptFriendRequest(Guid senderId, Guid receiverId)
        {
            var res = await _userService.AcceptFriendRequest(senderId, receiverId);
            if (!res.accepted)
            {
                return BadRequest(res.errorMsg);
            }
            return Ok();
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> DenyFriendRequest(Guid senderId, Guid receiverId)
        {
            var res = await _userService.DenyFriendRequest(senderId, receiverId);
            if (!res.declined)
            {
                return BadRequest(res.errorMsg);
            }
            return Ok();
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ShowPendingFriendRequests(Guid userId)
        {
            return new JsonResult(await _userService.ShowAllPendingFriendRequests(userId));
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ShowAcceptedFriendRequests(Guid userId)
        {
            return new JsonResult(await _userService.ShowAllAcceptedFriendRequests(userId));
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ShowDeclinedFriendRequests(Guid userId)
        {
            return new JsonResult(await _userService.ShowAllDeclinedFriendRequests(userId));
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteFriendship(Guid senderId, Guid receiverId)
        {
            bool isDeleted = await _userService.DeleteFriendship(senderId, receiverId);
            if (!isDeleted)
            {
                return BadRequest("Friendship was not found");
            }
            return Ok();    
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<JsonResult> ShowReceivedMessages(Guid userId)
        {
            List<MessageModel>? messages = await _userService.ShowReceivedMessages(userId);
            return new JsonResult(messages);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendMessage(Guid senderId, Guid receiverId, string content)
        {
            var res = await _userService.SendMessage(senderId, receiverId, content);
            if(!res.isSent)
            {
                return BadRequest(res.errorMsg);
            }
            return Ok();
        }
    }
}
