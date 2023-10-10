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
    internal class PendingUserConfiguration : IEntityTypeConfiguration<PendingUser>
    {
        public void Configure(EntityTypeBuilder<PendingUser> modelBuilder)
        {

            modelBuilder
                .ToTable("Pending Users");
        }
    }
}
