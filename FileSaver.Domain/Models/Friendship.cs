namespace FileSaver.Domain.Models
{
    using FileSaver.Domain.Enums;
    using FileSaver.Domain.Interfaces;

    public class Friendship : IEntity
    {
        public Guid Id { get; set; }

        public Guid SenderUserID { get; set; }

        public Guid ReceiverUserID { get; set; }

        public FriendshipStatus Status { get; set; }

        public virtual User SenderUser { get; set; }

        public virtual User ReceiverUser { get; set; }
    }
}
