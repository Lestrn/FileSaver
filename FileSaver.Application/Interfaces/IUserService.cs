﻿namespace FileSaver.Application.Interfaces
{
    using FileSaver.Domain.DTOs;
    using FileSaver.Domain.Enums;
    using FileSaver.Domain.Models;
    using FileSaver.Domain.Models.Mapping.Models;
    using Microsoft.AspNetCore.Http;

    public interface IUserService
    {
        public Task<bool> UpdateRole(Guid userId, UserRoles role);

        public Task<List<UserModel>> GetAllUsers();

        public Task<UserModel?> GetUserInfo(Guid userId);

        public Task<SavedFileModel?> GetFileInfo(Guid fileId);

        public Task<bool> UploadFile(Guid userId, IFormFile file);

        public Task<SavedFile?> GetFileById(Guid fileId);

        public Task<byte[]?> GetAvatarBytes(Guid userId);

        public Task<List<SavedFileModel>> GetOwnFiles(Guid userId);

        public Task<List<SavedFileModel>> GetReceivedFiles(Guid userId);

        public Task<List<SharedFileModel>> GetFilesThatUserShares(Guid userId);

        public Task<bool> DeleteFile(Guid fileId, Guid userId);

        public Task<(bool isUploaded, string errorMsg)> UploadAvatar(Guid userId, IFormFile image);

        public Task<(bool isChanged, string message)> ChangePassword(Guid userId, string newPassoword);

        public Task<(bool isShared, string errorMsg)> ShareFile(UserFileShareDTO userShare);

        public Task<(bool isStopped, string errorMsg)> StopSharing(UserFileShareDTO userShare);

        public Task<(bool isSent, string errorMsg)> SendFriendRequest(Guid senderId, string username);

        public Task<(bool accepted, string errorMsg)> AcceptFriendRequest(Guid senderId, Guid receiverId);

        public Task<(bool declined, string errorMsg)> DenyFriendRequest(Guid senderId, Guid receiverId);

        public Task<List<FriendshipModel>> GetAllPendingFriendRequests(Guid userId);

        public Task<List<FriendModel>?> GetAllAcceptedFriendRequests(Guid userId);

        public Task<List<FriendshipModel>> GetAllDeclinedFriendRequests(Guid userId);

        public Task<bool> DeleteFriendship(Guid senderId, Guid receiverId);

        public Task<List<MessageModel>?> GetReceivedMessages(Guid userid);

        public Task<List<MessageModel>?> GetSentMessages(Guid userid);

        public Task<(bool isSent, string errorMsg)> SendMessage(Guid senderId, Guid receiverId, string content);

        public Task<(bool isDeleted, string errorMsg)> DeleteMessage(Guid msgId);
    }
}
