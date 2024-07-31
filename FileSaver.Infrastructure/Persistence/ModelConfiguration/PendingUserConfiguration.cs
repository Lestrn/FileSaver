namespace FileSaver.Infrastructure.Persistence.Modelconfiguration
{
    using FileSaver.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class PendingUserConfiguration : IEntityTypeConfiguration<PendingUser>
    {
        public void Configure(EntityTypeBuilder<PendingUser> modelBuilder)
        {
            modelBuilder
                .ToTable("Pending Users");
        }
    }
}
