using FileSaver.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Models.Mapping.Models
{
    public class UserModelEmailRole
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; }
    }
}
