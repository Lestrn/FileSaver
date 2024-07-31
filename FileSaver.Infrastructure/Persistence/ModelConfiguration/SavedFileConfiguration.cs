namespace FileSaver.Infrastructure.Persistence.Modelconfiguration
{
    using FileSaver.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class SavedFileConfiguration : IEntityTypeConfiguration<SavedFile>
    {
        public void Configure(EntityTypeBuilder<SavedFile> modelBuilder)
        {
            modelBuilder
                .ToTable("Saved Files");
        }
    }
}
