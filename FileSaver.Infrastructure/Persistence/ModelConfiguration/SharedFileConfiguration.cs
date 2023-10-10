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
    internal class SharedFileConfiguration : IEntityTypeConfiguration<SharedFile>
    {
        public void Configure(EntityTypeBuilder<SharedFile> modelBuilder)
        {

            modelBuilder
                .ToTable("Shared Files");

            modelBuilder
                .HasOne(sf => sf.File)
                .WithMany()
                .HasForeignKey(sf => sf.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(sf => sf.SharedByUser)
                .WithMany(u => u.SharedFiles)
                .HasForeignKey(sf => sf.SharedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(sf => sf.SharedWithUser)
                .WithMany()
                .HasForeignKey(sf => sf.SharedWithUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
