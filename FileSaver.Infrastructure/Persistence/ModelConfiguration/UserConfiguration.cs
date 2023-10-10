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
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder
                .ToTable("Users")
                .HasMany(user => user.Files)
                .WithOne(file => file.User)
                .HasForeignKey(file => file.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
