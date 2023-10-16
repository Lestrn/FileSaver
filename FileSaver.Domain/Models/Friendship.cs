using FileSaver.Domain.Enums;
using FileSaver.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Models
{
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
