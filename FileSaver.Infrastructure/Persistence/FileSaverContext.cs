using FileSaver.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Infrastructure.Persistence
{
    public class FileSaverContext : DbContext
    {
        public FileSaverContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<UserDbModel> User { get; set; }
        public DbSet<FileDbModel> File { get; set; }
        public DbSet<UnconfirmedUserDbModel> UnconfirmedUsers { get; set; }
    }
}
