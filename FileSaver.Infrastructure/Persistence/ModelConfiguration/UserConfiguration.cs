namespace FileSaver.Infrastructure.Persistence.Modelconfiguration
{
    using FileSaver.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
