using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {


            builder.HasOne(u => u.RequestorByUser)
           .WithMany()
           .HasForeignKey(u => u.RequestorBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ApproverByUser)
           .WithMany()
           .HasForeignKey(u => u.ApproverBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
