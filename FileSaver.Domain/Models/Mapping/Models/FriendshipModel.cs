using FileSaver.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Models.Mapping.Models
{
    public class FriendshipModel
    {
        public Guid SenderUserID { get; set; }
        public Guid ReceiverUserID { get; set; }
    }
}
