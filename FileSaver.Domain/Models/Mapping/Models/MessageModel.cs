namespace FileSaver.Domain.Models.Mapping.Models
{
    public class MessageModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid SenderUserID { get; set; }

        public Guid ReceiverUserID { get; set; }
    }
}
