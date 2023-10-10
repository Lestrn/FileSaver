using FileSaver.Domain.Models;
using FileSaver.Infrastructure.Persistence.Modelconfiguration;
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
        public DbSet<User> Users { get; set; }
        public DbSet<SavedFile> Files { get; set; }
        public DbSet<PendingUser> PendingUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {       
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new SavedFileConfiguration());
            modelBuilder.ApplyConfiguration(new PendingUserConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new SharedFileConfiguration());
            modelBuilder.ApplyConfiguration(new FriendshipConfiguration());
        }

    }
}
