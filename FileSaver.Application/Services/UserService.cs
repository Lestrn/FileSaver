using FileSaver.Application.Interfaces;
using FileSaver.Domain.DTOs;
using FileSaver.Domain.Enums;
using FileSaver.Domain.Interfaces;
using FileSaver.Domain.Models;
using Microsoft.AspNetCore.Http;
using FileSaver.Domain.Resources;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using AutoMapper;
using FileSaver.Domain.Models.Mapping.Models;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace FileSaver.Application.Services
{
    public class UserService : IUserService
    {
        private IEntityRepository<User> _userRepository;
        private IEntityRepository<SavedFile> _fileRepository;
        private IEntityRepository<SharedFile> _sharedFileRepository;
        private IMapper _mapper;
        public UserService(IEntityRepository<User> userRepository, IEntityRepository<SavedFile> fileRepository, IEntityRepository<SharedFile> sharedFileRepositor, IMapper mapper)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
            _sharedFileRepository = sharedFileRepositor;
            _mapper = mapper;
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
        public async Task<List<UserModelEmailRole>> GetAllUsers()
        {
            List<User> users = await _userRepository.GetAllAsync();
            List<UserModelEmailRole> userModel = new List<UserModelEmailRole>(users.Count);
            users.ForEach(user => userModel.Add(_mapper.Map<UserModelEmailRole>(user)));
            return userModel;
        }
        public async Task<SavedFile?> GetFileById(Guid fileId)
        {
            return await _fileRepository.FindByIdAsync(fileId);
        }
        public async Task<List<SavedFileModel>> GetAllFilesByUserId(Guid userId)
        {

            User userDb = await _userRepository.FindByIdWithIncludesAsync(userId, UserProperties.Files.ToString());
            List<SavedFile> userDbFiles = userDb.Files;
            List<SavedFileModel> userFiles = new List<SavedFileModel>(userDbFiles.Count);
            foreach (var file in userDbFiles)
            {             
                userFiles.Add(_mapper.Map<SavedFileModel>(file));
            }
            List<SavedFile> receivedFiles = (await _sharedFileRepository.Where(sf => sf.SharedWithUserId == userId)).Select(sf => sf.File).ToList();
            foreach (var file in receivedFiles)
            {
                userFiles.Add(_mapper.Map<SavedFileModel>(file));
            }
            return userFiles;
        }
        public async Task<bool> DeleteFile(Guid fileId)
        {
            SavedFile fileDb = await _fileRepository.FindByIdAsync(fileId);
            if (fileDb == null)
            {
                return false;
            }
            _fileRepository.DeleteAsync(fileDb);
            await _fileRepository.SaveChangesAsync();
            return true;
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
        public async Task<bool> DeleteAccount(UserDTODelete user)
        {
            User? userDb = await _userRepository.FindByIdWithIncludesAsync(user.Id);
            if (userDb == null)
            {
                return false;
            }
             _userRepository.DeleteAsync(userDb);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ShareFile(UserFileShareDTO userShare)
        {
            User? fileOwnerDb = await _userRepository.FindByIdWithIncludesAsync(userShare.OwnerId, UserProperties.Files.ToString(), UserProperties.SharedFiles.ToString());
            if (fileOwnerDb == null)
            {
                return false;
            }
            SavedFile sharedFile = fileOwnerDb.Files.Where(file => file.Id == userShare.FileId).FirstOrDefault();
            if (sharedFile == null)
            {
                return false;
            }
            User? sharedWith = await _userRepository.FindByIdAsync(userShare.SharedWithId);
            if (sharedWith == null || fileOwnerDb.SharedFiles == null)
            {
                return false;
            }
            fileOwnerDb.SharedFiles.Add(new SharedFile { File = sharedFile, SharedByUser = fileOwnerDb, SharedWithUser = sharedWith });
            await _fileRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UploadFile(Guid userId, IFormFile file)
        {
            var res = await UploadFileGeneral(userId, file);
            return res.isUploaded;
        }

        public async Task<(bool isUploaded, string errorMsg)> UploadAvatar(Guid userId, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return (false, "Invalid file");
            }
            if (!IsImage(image.ContentType))
            {
                return (false, "Only image files are allowed.");
            }
            var res = await UploadFileGeneral(userId, image, true);
            return (res.isUploaded, res.errorMsg);
        }

        private bool IsImage(string contentType)
        {
            string[] allowedContentTypes = { "image/jpeg", "image/png", "image/bmp" };
            return allowedContentTypes.Contains(contentType);
        }
        private async Task<(bool isUploaded, string errorMsg)> UploadFileGeneral(Guid userId, IFormFile file, bool isAvatar = false)
        {
            User? dbUser = await _userRepository.FindByIdWithIncludesAsync(userId, UserProperties.Files.ToString());
            if (dbUser == null)
            {
                return (false, "User with such id was not found");
            }
            if (file == null || file.Length == 0)
            {
                return (false, "File was empty or null");
            }
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                Domain.Models.SavedFile fileDb = new SavedFile()
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Content = stream.ToArray()
                };
                if (!isAvatar)
                {
                    dbUser.Files.Add(fileDb);
                }
                else
                {
                    await file.CopyToAsync(stream);
                    byte[] imageBytes = stream.ToArray();
                    dbUser.Image = imageBytes;
                }
                await _userRepository.UpdateAsync(dbUser);
                await _userRepository.SaveChangesAsync();
            }
            return (true, string.Empty);
        }
    }
}
