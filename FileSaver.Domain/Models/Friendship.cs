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
        public Guid UserID1 { get; set; }
        public Guid UserID2 { get; set; }
        public FriendshipStatus Status { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
