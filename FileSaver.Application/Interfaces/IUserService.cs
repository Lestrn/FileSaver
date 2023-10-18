﻿using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Models;
using FileSaver.Domain.Models.Mapping.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace FileSaver.Application.Interfaces
{
    public interface IUserService
    {
        public Task<bool> UpdateRole(Guid userId, UserRoles role);
        public Task<List<UserModelEmailRole>> GetAllUsers();
        public Task<bool> UploadFile(Guid userId, IFormFile file);
        public Task<SavedFile?> GetFileById(Guid fileId);
        public Task<List<SavedFileModel>> GetAllFilesByUserId(Guid userId);
        public Task<bool> DeleteFile(Guid fileId);
        public Task<(bool isUploaded, string errorMsg)> UploadAvatar(Guid userId, IFormFile image);
        public Task<(bool isChanged, string message)> ChangePassword(Guid userId, string newPassoword);
        public Task<bool> DeleteAccount(UserDTODelete user);
        public Task<bool> ShareFile(UserFileShareDTO userShare);
        public Task<(bool isSent, string errorMsg)> SendFriendRequest(Guid senderId, string username);
        public Task<(bool accepted, string errorMsg)> AcceptFriendRequest(Guid senderId, Guid receiverId);
        public Task<(bool declined, string errorMsg)> DenyFriendRequest(Guid senderId, Guid receiverId);
        public Task<List<FriendshipModel>> ShowAllPendingFriendRequests(Guid userId);
        public Task<List<FriendshipModel>?> ShowAllAcceptedFriendRequests(Guid userId);
        public Task<List<FriendshipModel>> ShowAllDeclinedFriendRequests(Guid userId);
        public Task<bool> DeleteFriendship(Guid senderId, Guid receiverId);
        public Task<List<MessageModel>?> ShowReceivedMessages(Guid userid);
        public Task<(bool isSent, string errorMsg)> SendMessage(Guid senderId, Guid receiverId, string content);
    }
}
