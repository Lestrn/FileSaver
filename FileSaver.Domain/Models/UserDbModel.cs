﻿using FileSaver.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileSaver.Domain.Models
{
    [Table("Users")]
    public class UserDbModel : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public byte[]? Image { get; set; }
        public List<FileDbModel> Files { get; set; }
    }
}