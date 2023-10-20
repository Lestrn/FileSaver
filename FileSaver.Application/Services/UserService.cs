namespace FileSaver.Application.Services
{
    using System.Text.RegularExpressions;
    using AutoMapper;
    using FileSaver.Application.Interfaces;
    using FileSaver.Domain.DTOs;
    using FileSaver.Domain.Enums;
    using FileSaver.Domain.Interfaces;
    using FileSaver.Domain.Models;
    using FileSaver.Domain.Models.Mapping.Models;
    using FileSaver.Domain.Resources;
    using Microsoft.AspNetCore.Http;

    public class UserService : IUserService
    {
        private readonly IEntityRepository<User> userRepository;
        private readonly IEntityRepository<SavedFile> fileRepository;
        private readonly IEntityRepository<SharedFile> sharedFileRepository;
        private readonly IEntityRepository<Friendship> friendshipRepository;
        private readonly IEntityRepository<Message> messageRepository;
        private readonly IMapper mapper;

        public UserService(
            IEntityRepository<User> userRepository,
            IEntityRepository<SavedFile> fileRepository,
            IEntityRepository<SharedFile> sharedFileRepositor,
            IMapper mapper,
            IEntityRepository<Friendship> friendshipRepository,
            IEntityRepository<Message> messageRepository)
        {
            this.userRepository = userRepository;
            this.fileRepository = fileRepository;
            this.sharedFileRepository = sharedFileRepositor;
            this.mapper = mapper;
            this.friendshipRepository = friendshipRepository;
            this.messageRepository = messageRepository;
        }

        public async Task<bool> UpdateRole(Guid userId, UserRoles role)
        {
            User? userDb = await this.userRepository.FindByIdAsync(userId);
            if (userDb == null)
            {
                return false;
            }

            userDb.Role = role;
            await this.userRepository.UpdateAsync(userDb);
            await this.userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserModelEmailRole>> GetAllUsers()
        {
            List<User> users = await this.userRepository.GetAllAsync();
            List<UserModelEmailRole> userModel = new List<UserModelEmailRole>(users.Count);
            users.ForEach(user => userModel.Add(this.mapper.Map<UserModelEmailRole>(user)));
            return userModel;
        }

        public async Task<SavedFile?> GetFileById(Guid fileId)
        {
            return await this.fileRepository.FindByIdAsync(fileId);
        }

        public async Task<List<SavedFileModel>> GetAllFilesByUserId(Guid userId)
        {
            User? userDb = await this.userRepository.FindByIdWithIncludesAsync(userId, UserProperties.Files.ToString());
            if (userDb == null || userDb.Files == null)
            {
                return new List<SavedFileModel>();
            }

            List<SavedFile> userDbFiles = userDb.Files;
            List<SavedFileModel> userFiles = new List<SavedFileModel>(userDbFiles.Count);
            foreach (var file in userDbFiles)
            {
                userFiles.Add(this.mapper.Map<SavedFileModel>(file));
            }

            List<SharedFile> sharedFiles = (await this.sharedFileRepository.Where(sf => sf.SharedWithUserId == userId)).ToList();
            List<SharedFile?> sharedFilesWithIncludes = new List<SharedFile?>(sharedFiles.Count);
            for (int i = 0; i < sharedFiles.Count; i++)
            {
                sharedFilesWithIncludes.Add(await this.sharedFileRepository.FindByIdWithIncludesAsync(sharedFiles[i].Id, SharedFileProperties.File.ToString()));
            }

            List<SavedFile?> receivedFiles = sharedFilesWithIncludes.Select(sf => sf?.File).ToList();
            foreach (var file in receivedFiles)
            {
                userFiles.Add(this.mapper.Map<SavedFileModel>(file));
            }

            return userFiles;
        }

        public async Task<bool> DeleteFile(Guid fileId)
        {
            SavedFile? fileDb = await this.fileRepository.FindByIdAsync(fileId);
            if (fileDb == null)
            {
                return false;
            }

            this.fileRepository.Delete(fileDb);
            await this.fileRepository.SaveChangesAsync();
            return true;
        }

        public async Task<(bool isChanged, string message)> ChangePassword(Guid userId, string newPassoword)
        {
            User? userDbModel = await this.userRepository.FindByIdAsync(userId);
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
            await this.userRepository.UpdateAsync(userDbModel);
            await this.userRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool isShared, string errorMsg)> ShareFile(UserFileShareDTO userShare)
        {
            if (!await this.AreFriends(userShare.OwnerId, userShare.SharedWithId))
            {
                return (false, "These users are not friends");
            }

            User? fileOwnerDb = await this.userRepository.FindByIdWithIncludesAsync(userShare.OwnerId, UserProperties.Files.ToString(), UserProperties.SharedFiles.ToString());
            if (fileOwnerDb == null || fileOwnerDb.Files == null)
            {
                return (false, "Owner of file was not found");
            }

            SavedFile? sharedFile = fileOwnerDb.Files.Where(file => file.Id == userShare.FileId).FirstOrDefault();
            if (sharedFile == null)
            {
                return (false, "File was not  found");
            }

            User? sharedWith = await this.userRepository.FindByIdAsync(userShare.SharedWithId);
            if (sharedWith == null || fileOwnerDb.SharedFiles == null)
            {
                return (false, "Shared user was not found");
            }

            fileOwnerDb.SharedFiles.Add(new SharedFile { File = sharedFile, SharedByUser = fileOwnerDb, SharedWithUser = sharedWith });
            await this.fileRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool isStopped, string errorMsg)> StopSharing(UserFileShareDTO userShare)
        {
            SharedFile? sharedFile = (await this.sharedFileRepository.Where(sf => sf.SharedByUserId == userShare.OwnerId && sf.SharedWithUserId == userShare.SharedWithId && sf.FileId == userShare.FileId))
                .FirstOrDefault();
            if (sharedFile == null)
            {
                return (false, "Shared file was not found");
            }

            this.sharedFileRepository.Delete(sharedFile);
            await this.userRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<bool> UploadFile(Guid userId, IFormFile file)
        {
            User? dbUser = await this.userRepository.FindByIdWithIncludesAsync(userId, UserProperties.Files.ToString());
            if (dbUser == null || dbUser.Files == null)
            {
                return false;
            }

            var res = await this.GetFile(file);
            if (res.file == null)
            {
                return false;
            }

            dbUser.Files.Add(res.file);
            await this.userRepository.UpdateAsync(dbUser);
            await this.userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<(bool isUploaded, string errorMsg)> UploadAvatar(Guid userId, IFormFile image)
        {
            if (!this.IsImage(image.ContentType))
            {
                return (false, "Only image files are allowed.");
            }

            User? dbUser = await this.userRepository.FindByIdAsync(userId);
            if (dbUser == null)
            {
                return (false, "User with such id was not found");
            }

            var res = await this.GetFile(image);
            if (res.file == null)
            {
                return (false, res.errorMsg);
            }

            dbUser.Image = res.file.Content;
            await this.userRepository.UpdateAsync(dbUser);
            await this.userRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool isSent, string errorMsg)> SendFriendRequest(Guid senderId, string username) // If friend request was declined it cannot be sent again untill friendship deleted (only receiver can delete declined request)
        {
            User? sender = await this.userRepository.FindByIdWithIncludesAsync(senderId, UserProperties.Friendships.ToString());
            if (sender == null)
            {
                return (false, "sender was not found");
            }

            User? receiver = (await this.userRepository.Where(user => user.Username == username)).FirstOrDefault();
            if (receiver == null)
            {
                return (false, "receiver was not found");
            }

            Guid receiverId = receiver.Id;
            bool friendshipExists = await this.friendshipRepository.Any(fs => (fs.SenderUserID == senderId && fs.ReceiverUserID == receiverId) || (fs.SenderUserID == receiverId && fs.ReceiverUserID == senderId));
            if (friendshipExists)
            {
                return (false, "friendship already exists, or friend request has already been sent");
            }

            Friendship friendship = new Friendship() { Status = FriendshipStatus.Pending, SenderUser = sender, ReceiverUser = receiver };
            this.friendshipRepository.Add(friendship);
            await this.userRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool accepted, string errorMsg)> AcceptFriendRequest(Guid senderId, Guid receiverId)
        {
           var res = await this.SetFriendshipStatus(senderId, receiverId, FriendshipStatus.Accepted);
           return (res.completed, res.errorMsg);
        }

        public async Task<(bool declined, string errorMsg)> DenyFriendRequest(Guid senderId, Guid receiverId)
        {
            var res = await this.SetFriendshipStatus(senderId, receiverId, FriendshipStatus.Declined);
            return (res.completed, res.errorMsg);
        }

        public async Task<List<FriendshipModel>> ShowAllPendingFriendRequests(Guid userId) // Only receiver of friend request sees
        {
            return await this.ShowFriends(userId, FriendshipStatus.Pending);
        }

        public async Task<List<FriendshipModel>> ShowAllDeclinedFriendRequests(Guid userId) // Only receiver of friend request sees
        {
            return await this.ShowFriends(userId, FriendshipStatus.Declined);
        }

        public async Task<List<FriendshipModel>?> ShowAllAcceptedFriendRequests(Guid userId) // Receiver and Sender both see
        {
            List<Friendship> friendships = (await this.friendshipRepository.Where(fs => (fs.SenderUserID == userId || fs.ReceiverUserID == userId) && fs.Status == FriendshipStatus.Accepted)).ToList();
            List<FriendshipModel> friendshipsModel = new List<FriendshipModel>(friendships.Count);
            friendships.ForEach(fs =>
            {
                friendshipsModel.Add(this.mapper.Map<FriendshipModel>(fs));
            });
            return friendshipsModel;
        }

        public async Task<bool> DeleteFriendship(Guid senderId, Guid receiverId)
        {
            Friendship? friendship = (await this.friendshipRepository.Where(fs => fs.SenderUserID == senderId && fs.ReceiverUserID == receiverId)).FirstOrDefault();
            if (friendship == null)
            {
                return false;
            }

            this.friendshipRepository.Delete(friendship);
            await this.friendshipRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<MessageModel>?> ShowReceivedMessages(Guid userid)
        {
            User? receiver = await this.userRepository.FindByIdWithIncludesAsync(userid, UserProperties.ReceivedMessages.ToString());
            if (receiver == null)
            {
                return null;
            }

            List<Message> messages = receiver.ReceivedMessages.ToList();
            List<MessageModel> messagesModel = new List<MessageModel>(messages.Count);
            messages.ForEach(msg => messagesModel.Add(this.mapper.Map<MessageModel>(msg)));
            return messagesModel;
        }

        public async Task<List<MessageModel>?> ShowSentMessages(Guid userid)
        {
            User? sender = await this.userRepository.FindByIdWithIncludesAsync(userid, UserProperties.SentMessages.ToString());
            if (sender == null)
            {
                return null;
            }

            List<Message> messages = sender.SentMessages.ToList();
            List<MessageModel> messagesModel = new List<MessageModel>(messages.Count);
            messages.ForEach(msg =>
            messagesModel.Add(this.mapper.Map<MessageModel>(msg)));
            return messagesModel;
        }

        public async Task<(bool isSent, string errorMsg)> SendMessage(Guid senderId, Guid receiverId, string content)
        {
            User? sender = await this.userRepository.FindByIdWithIncludesAsync(senderId, UserProperties.SentMessages.ToString());
            User? receiver = await this.userRepository.FindByIdWithIncludesAsync(receiverId, UserProperties.ReceivedMessages.ToString());
            if (sender == null)
            {
                return (false, "Sender id was invalid");
            }

            if (receiver == null)
            {
                return (false, "Receiver id was invalid");
            }

            if (!await this.AreFriends(senderId, receiverId))
            {
                return (false, "Users are not friends");
            }

            Message message = new Message { Content = content, Sender = sender, Receiver = receiver, Timestamp = DateTime.Now };
            sender.SentMessages.Add(message);
            receiver.ReceivedMessages.Add(message);
            await this.userRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool isDeleted, string errorMsg)> DeleteMessage(Guid msgId)
        {
            Message? message = await this.messageRepository.FindByIdAsync(msgId);
            if (message == null)
            {
                return (false, "message was not found");
            }

            this.messageRepository.Delete(message);
            await this.messageRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        private async Task<List<FriendshipModel>> ShowFriends(Guid userId, FriendshipStatus status) // General method for receiver
        {
            List<Friendship> friendships = (await this.friendshipRepository.Where(fs => fs.ReceiverUserID == userId && fs.Status == status)).ToList();
            List<FriendshipModel> friendshipsModel = new List<FriendshipModel>(friendships.Count);
            friendships.ForEach(fs =>
            {
                friendshipsModel.Add(this.mapper.Map<FriendshipModel>(fs));
            });
            return friendshipsModel;
        }

        private async Task<(bool completed, string errorMsg)> SetFriendshipStatus(Guid senderId, Guid receiverId, FriendshipStatus status)
        {
            Friendship? friendship = (await this.friendshipRepository.Where(fs => fs.ReceiverUserID == receiverId && fs.SenderUserID == senderId)).FirstOrDefault();
            if (friendship == null)
            {
                return (false, "sent request was not found");
            }

            friendship.Status = status;
            await this.friendshipRepository.UpdateAsync(friendship);
            await this.friendshipRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        private bool IsImage(string contentType)
        {
            string[] allowedContentTypes = { "image/jpeg", "image/png", "image/bmp" };
            return allowedContentTypes.Contains(contentType);
        }

        private async Task<(SavedFile? file, string errorMsg)> GetFile(IFormFile file)
        {
            SavedFile? savedFile = null;
            if (file == null || file.Length == 0)
            {
                return (savedFile, "File was empty or null");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                savedFile = new SavedFile()
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Content = stream.ToArray(),
                };
            }

            return (savedFile, string.Empty);
        }

        private async Task<bool> AreFriends(Guid user1, Guid user2)
        {
            bool areFriends = await this.friendshipRepository
                 .Any(fs => (fs.SenderUserID == user1 && fs.ReceiverUserID == user2) || (fs.SenderUserID == user2 && fs.ReceiverUserID == user1) && fs.Status == FriendshipStatus.Accepted);
            return areFriends;
        }
    }
}
