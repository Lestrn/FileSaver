namespace FileSaver.API.Controllers
{
    using System.Security.Claims;
    using FileSaver.Application.Interfaces;
    using FileSaver.Domain.DTOs;
    using FileSaver.Domain.Enums;
    using FileSaver.Domain.Models;
    using FileSaver.Domain.Models.Mapping.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> DownloadFile(Guid fileId, Guid ownerId)
        {
            if (!await this.ClaimsAreEqualToInput(ownerId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            SavedFile? file = await this.userService.GetFileById(fileId);
            if (file != null)
            {
                var contentDisposition = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.FileName,
                    Inline = false,
                };

                this.Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
                return this.File(file.Content, file.ContentType);
            }

            return this.NotFound();
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            List<UserModel> userModels = await this.userService.GetAllUsers();
            return this.Ok(new JsonResult(userModels));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserInfo(Guid userId)
        {
            UserModel? userModel = await this.userService.GetUserInfo(userId);
            if (userModel == null)
            {
                return this.BadRequest("User was not found");
            }

            return this.Ok(new JsonResult(userModel));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllFilesByUserId(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            List<SavedFileModel> fileDbModels = await this.userService.GetAllFilesByUserId(userId);
            return this.Ok(new JsonResult(fileDbModels));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetFilesThatUserShares(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            List<SharedFileModel> sharedFiles = await this.userService.GetFilesThatUserShares(userId);
            return this.Ok(new JsonResult(sharedFiles));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPendingFriendRequests(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            return this.Ok(new JsonResult(await this.userService.GetAllPendingFriendRequests(userId)));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAcceptedFriendRequests(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            return this.Ok(new JsonResult(await this.userService.GetAllAcceptedFriendRequests(userId)));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDeclinedFriendRequests(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            return this.Ok(new JsonResult(await this.userService.GetAllDeclinedFriendRequests(userId)));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetReceivedMessages(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            List<MessageModel>? messages = await this.userService.GetReceivedMessages(userId);
            return this.Ok(new JsonResult(messages));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSentMessages(Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            List<MessageModel>? messages = await this.userService.GetSentMessages(userId);
            return this.Ok(new JsonResult(messages));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFile(Guid userId, IFormFile file)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            return await this.userService.UploadFile(userId, file) ? this.Ok() : this.BadRequest();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ShareFile(UserFileShareDTO share)
        {
            if (!await this.ClaimsAreEqualToInput(share.OwnerId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.ShareFile(share);
            if (!res.isShared)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendMessage(Guid senderId, Guid receiverId, string content)
        {
            if (!await this.ClaimsAreEqualToInput(senderId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.SendMessage(senderId, receiverId, content);
            if (!res.isSent)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(Guid userId, string role)
        {
            bool roleIsParsed = Enum.TryParse(typeof(UserRoles), role, out object? roleEnum);
            if (!roleIsParsed)
            {
                return this.BadRequest("incorrect role input");
            }

            bool isUpdated = await this.userService.UpdateRole(userId, (UserRoles)roleEnum);
            if (!isUpdated)
            {
                return this.BadRequest("User with such id wasnt found");
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UploadAvatar(Guid userId, IFormFile image)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var uploadResult = await this.userService.UploadAvatar(userId, image);
            if (!uploadResult.isUploaded)
            {
                return this.BadRequest(uploadResult.errorMsg);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> ChangePassword(Guid userId, string newPassword)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.ChangePassword(userId, newPassword);
            if (!res.isChanged)
            {
                return this.BadRequest(res.message);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> SendFriendRequest(Guid senderid, string receiverUsername)
        {
            if (!await this.ClaimsAreEqualToInput(senderid))
            {
                return this.BadRequest("Credentials are invalid");
            }

            if (this.User.FindFirst(ClaimTypes.Name).Value == receiverUsername)
            {
                return this.BadRequest("You cant send friend request to yourself!");
            }

            var res = await this.userService.SendFriendRequest(senderid, receiverUsername);
            if (!res.isSent)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> AcceptFriendRequest(Guid senderId, Guid receiverId)
        {
            if (!await this.ClaimsAreEqualToInput(receiverId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.AcceptFriendRequest(senderId, receiverId);
            if (!res.accepted)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> DenyFriendRequest(Guid senderId, Guid receiverId)
        {
            if (!await this.ClaimsAreEqualToInput(receiverId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.DenyFriendRequest(senderId, receiverId);
            if (!res.declined)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> StopSharingFile(UserFileShareDTO share)
        {
            if (!await this.ClaimsAreEqualToInput(share.OwnerId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.StopSharing(share);
            if (!res.isStopped)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteFileById(Guid fileId, Guid ownerId)
        {
            if (!await this.ClaimsAreEqualToInput(ownerId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            return await this.userService.DeleteFile(fileId) ? this.Ok() : this.NotFound();
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteFriendship(Guid senderId, Guid receiverId)
        {
            if (!(await this.ClaimsAreEqualToInput(senderId) ^ await this.ClaimsAreEqualToInput(receiverId)))
            {
                return this.BadRequest("Credentials are invalid");
            }

            bool isDeleted = await this.userService.DeleteFriendship(senderId, receiverId);
            if (!isDeleted)
            {
                return this.BadRequest("Friendship was not found");
            }

            return this.Ok();
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteMessage(Guid msgId, Guid userId)
        {
            if (!await this.ClaimsAreEqualToInput(userId))
            {
                return this.BadRequest("Credentials are invalid");
            }

            var res = await this.userService.DeleteMessage(msgId);
            if (!res.isDeleted)
            {
                return this.BadRequest(res.errorMsg);
            }

            return this.Ok();
        }

        private Task<bool> ClaimsAreEqualToInput(Guid userId)
        {
            Guid userIdFromToken = Guid.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (userIdFromToken != userId)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
