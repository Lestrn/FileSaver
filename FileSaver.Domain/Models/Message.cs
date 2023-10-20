namespace FileSaver.Domain.Models
{
    using FileSaver.Domain.Interfaces;

    public class Message : IEntity
    {
        public Guid Id { get; set; }

        public Guid SenderUserID { get; set; }

        public Guid ReceiverUserID { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public virtual User Sender { get; set; }

        public virtual User Receiver { get; set; }
    }
}
