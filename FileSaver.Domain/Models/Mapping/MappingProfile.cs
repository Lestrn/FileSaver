namespace FileSaver.Domain.Models.Mapping
{
    using AutoMapper;
    using FileSaver.Domain.Models.Mapping.Models;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<SavedFile, SavedFileModel>();
            this.CreateMap<User, UserModel>();
            this.CreateMap<Friendship, FriendshipModel>();
            this.CreateMap<Message, MessageModel>();
            this.CreateMap<SharedFile, SharedFileModel>();
        }
    }
}
