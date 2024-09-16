using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketTransactionNotificationConfiguration : IEntityTypeConfiguration<TicketTransactionNotification>
    {
        public void Configure(EntityTypeBuilder<TicketTransactionNotification> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ReceiveByUser)
           .WithMany()
           .HasForeignKey(u => u.ReceiveBy)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
