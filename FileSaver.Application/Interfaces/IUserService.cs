using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FileSaver.Application.Interfaces
{
    public interface IUserService
    {
        public Task<bool> UpdateRole(Guid userId, UserRoles role);
        public Task<List<UserDTOEmailRole>> GetAllUsers();
        public Task<bool> UploadFile(Guid userId, IFormFile file);
        public Task<Domain.Models.File?> GetFileById(Guid fileId);
        public Task<List<FileDTO>> GetAllFilesByUserId(Guid userId);
        public Task<bool> DeleteFile(Guid fileId);
        public Task<(bool isUploaded, string errorMsg)> UploadAvatar(Guid userId, IFormFile image);
        public Task<(bool isChanged, string message)> ChangePassword(Guid userId, string newPassoword);
    }
}
