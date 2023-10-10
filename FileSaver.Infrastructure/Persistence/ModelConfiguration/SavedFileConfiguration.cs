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
    internal class SavedFileConfiguration : IEntityTypeConfiguration<SavedFile>
    {
        public void Configure(EntityTypeBuilder<SavedFile> modelBuilder)
        {
            modelBuilder
                .ToTable("Saved Files");
        }
    }
}
