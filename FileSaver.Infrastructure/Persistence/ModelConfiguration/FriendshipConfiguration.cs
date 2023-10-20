namespace FileSaver.Infrastructure.Persistence.Modelconfiguration
{
    using FileSaver.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> modelBuilder)
        {
            modelBuilder
             .HasOne(f => f.SenderUser)
             .WithMany(u => u.Friendships)
             .HasForeignKey(f => f.SenderUserID)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(f => f.ReceiverUser)
                .WithMany()
                .HasForeignKey(f => f.ReceiverUserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
