using FileSaver.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FileSaver.Infrastructure.Persistence.Modelconfiguration
{
    internal class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> modelBuilder)
        {
            modelBuilder
             .HasOne(f => f.User1)
             .WithMany(u => u.Friendships)
             .HasForeignKey(f => f.UserID1)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(f => f.User2)
                .WithMany()
                .HasForeignKey(f => f.UserID2)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
