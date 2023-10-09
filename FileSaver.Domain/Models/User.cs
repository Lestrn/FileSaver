﻿using FileSaver.Domain.Enums;
using FileSaver.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileSaver.Domain.Models
{
    [Table("Users")]
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRoles Role { get; set; }
        public byte[]? Image { get; set; }
        public List<File> Files { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<Friendship> Friendships { get; set; }
    }
}