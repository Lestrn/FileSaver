using FileSaver.Application.Interfaces;
using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Interfaces;
using FileSaver.Domain.Models;
using Microsoft.AspNetCore.Http;
using FileSaver.Domain.Resources;
using System.Text.RegularExpressions;
namespace FileSaver.Application.Services
{
    public class UserService : IUserService
    {
        private IEntityRepository<User> _userRepository;
        private IEntityRepository<Domain.Models.File> _fileRepository;

        public UserService(IEntityRepository<User> userRepository, IEntityRepository<Domain.Models.File> fileRepository)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
        }
        public async Task<bool> UpdateRole(Guid userId, UserRoles role)
        {
            User userDb = await _userRepository.FindByIdAsync(userId);
            if (userDb == null)
            {
                return false;
            }
            userDb.Role = role;
            await _userRepository.UpdateAsync(userDb);
            await _userRepository.SaveChangesAsync();

            return true;
        }
        public async Task<List<UserDTOEmailRole>> GetAllUsers()
        {
            List<UserDTOEmailRole> userDTOEmailRoles = (await _userRepository.Select(userDb => new UserDTOEmailRole() { Id = userDb.Id, Email = userDb.Email, Role = userDb.Role })).ToList();
            return userDTOEmailRoles;
        }
        public async Task<bool> UploadFile(Guid userId, IFormFile file)
        {
            User? dbUser = await _userRepository.FindByIdWithIncludesAsync(userId, "Files");
            if (dbUser == null)
            {
                return false;
            }
            if (file == null || file.Length == 0)
            {
                return false;
            }
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                Domain.Models.File fileDb = new Domain.Models.File()
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Content = stream.ToArray()
                };

                dbUser.Files.Add(fileDb);
                await _userRepository.UpdateAsync(dbUser);
                await _userRepository.SaveChangesAsync();
            }
            return true;
        }

        public async Task<Domain.Models.File?> GetFileById(Guid fileId)
        {
            return await _fileRepository.FindByIdAsync(fileId);
        }
        public async Task<List<FileDTO>> GetAllFilesByUserId(Guid userId)
        {
            List<Domain.Models.File> userDbFiles = (await _userRepository.FindByIdWithIncludesAsync(userId, "Files")).Files;
            List<FileDTO> userFiles = new List<FileDTO>(userDbFiles.Count);
            foreach (var file in userDbFiles)
            {
                userFiles.Add(new FileDTO() { ContentType = file.ContentType, FileName = file.FileName, Id = file.Id });
            }
            return userFiles;
        }
        public async Task<bool> DeleteFile(Guid fileId)
        {
            Domain.Models.File fileDb = await _fileRepository.FindByIdAsync(fileId);
            if (fileDb == null)
            {
                return false;
            }
            await _fileRepository.DeleteAsync(fileDb);
            await _fileRepository.SaveChangesAsync();
            return true;
        }
        public async Task<(bool isUploaded, string errorMsg)> UploadAvatar(Guid userId, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return (false, "Invalid file");
            }

            User userDbModel = await _userRepository.FindByIdAsync(userId);
            if (userDbModel == null)
            {
                return (false, "Invalid user id");
            }
            if (!IsImage(image.ContentType))
            {
                return (false, "Only image files are allowed.");
            }
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    userDbModel.Image = imageBytes;
                    await _userRepository.UpdateAsync(userDbModel);
                    await _userRepository.SaveChangesAsync();
                    return (true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return (true, $"Internal server error: {ex}");
            }

        }
        public async Task<(bool isChanged, string message)> ChangePassword(Guid userId, string newPassoword)
        {
            User? userDbModel = await _userRepository.FindByIdAsync(userId);
            if (userDbModel == null)
            {
                return (false, "User with such id was not found");
            }
            bool passwordIsValid = Regex.IsMatch(newPassoword, "(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}");
            if (!passwordIsValid)
            {
                return (false, ResourceMsgs.InvalidPasswordMsg);
            }
            userDbModel.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassoword, 13);
            await _userRepository.UpdateAsync(userDbModel);
            await _userRepository.SaveChangesAsync();
            return (true, string.Empty);
        }
        private bool IsImage(string contentType)
        {
            string[] allowedContentTypes = { "image/jpeg", "image/png", "image/bmp" };
            return allowedContentTypes.Contains(contentType);
        }
    }
}
