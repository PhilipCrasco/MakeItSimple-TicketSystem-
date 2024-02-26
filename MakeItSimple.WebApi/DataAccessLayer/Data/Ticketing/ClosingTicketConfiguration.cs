using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class ClosingTicketConfiguration : IEntityTypeConfiguration<ClosingTicket>
    {
        public void Configure(EntityTypeBuilder<ClosingTicket> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ClosedByUser)
           .WithMany()
           .HasForeignKey(u => u.ClosedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.RejectClosedByUser)
           .WithMany()
           .HasForeignKey(u => u.RejectClosedBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }

}
