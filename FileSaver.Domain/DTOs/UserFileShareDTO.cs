using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.DTOs
{
    public class UserFileShareDTO
    {
        public Guid OwnerId { get; set; }
        public Guid SharedWithId { get; set; }
        public Guid FileId { get; set; }
    }
}
