using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketReDateConfiguration : IEntityTypeConfiguration<TicketReDate>
    {
        public void Configure(EntityTypeBuilder<TicketReDate> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ReDateByUser)
           .WithMany()
           .HasForeignKey(u => u.ReDateBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.RejectReDateByUser)
           .WithMany()
           .HasForeignKey(u => u.RejectReDateBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
