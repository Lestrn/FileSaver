using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Models
{
    [Table("Messages")]
    public class Message
    {
        public Guid MessageID { get; set; }
        public Guid SenderUserID { get; set; }
        public Guid ReceiverUserID { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    }
}
