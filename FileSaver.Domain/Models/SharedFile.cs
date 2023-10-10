using FileSaver.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Models
{
    public class SharedFile : IEntity
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public SavedFile File { get; set; }
        public Guid SharedByUserId { get; set; } 
        public User SharedByUser { get; set; }
        public Guid SharedWithUserId { get; set; } 
        public User SharedWithUser { get; set; }
    }

}
