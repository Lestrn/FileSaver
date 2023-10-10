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
        public DbSet<User> User { get; set; }
        public DbSet<SavedFile> File { get; set; }
        public DbSet<PendingUser> UnconfirmedUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(user => user.Files)
                .WithOne(file => file.User) 
                .HasForeignKey(file => file.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SharedFile>()
                .HasOne(sf => sf.File)
                .WithMany()
                .HasForeignKey(sf => sf.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SharedFile>()
                .HasOne(sf => sf.SharedByUser)
                .WithMany(u => u.SharedFiles)
                .HasForeignKey(sf => sf.SharedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SharedFile>()
                .HasOne(sf => sf.SharedWithUser)
                .WithMany()
                .HasForeignKey(sf => sf.SharedWithUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserID1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.UserID2)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasKey(msg => msg.Id);

            modelBuilder.Entity<Message>()
                .HasOne(msg => msg.Sender)
                .WithMany(msg => msg.SentMessages)
                .HasForeignKey(msg => msg.SenderUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverUserID)
                .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);
        }

    }
}
