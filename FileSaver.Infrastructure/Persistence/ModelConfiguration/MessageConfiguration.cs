namespace FileSaver.Infrastructure.Persistence.Modelconfiguration
{
    using FileSaver.Domain.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> modelBuilder)
        {
            modelBuilder
            .HasKey(msg => msg.Id);

            modelBuilder
                .HasOne(msg => msg.Sender)
                .WithMany(msg => msg.SentMessages)
                .HasForeignKey(msg => msg.SenderUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverUserID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
