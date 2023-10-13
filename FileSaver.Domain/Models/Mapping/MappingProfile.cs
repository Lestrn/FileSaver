using AutoMapper;
using FileSaver.Domain.Models.Mapping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Domain.Models.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<SavedFile, SavedFileModel>();
            this.CreateMap<User, UserModelEmailRole>();
        }
    }
}
