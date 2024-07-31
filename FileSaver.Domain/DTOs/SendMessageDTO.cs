namespace FileSaver.Domain.DTOs
{
    public class SendMessageDTO
    {
       public Guid SenderId { get; set; }

       public Guid ReceiverId { get; set; }

       public string Content { get; set; }
    }
}
