using MakeItSimple.WebApi.Models.Ticketing.TicketRequest;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketConcernConfiguration : IEntityTypeConfiguration<TicketConcern>
    {
        public void Configure(EntityTypeBuilder<TicketConcern> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ClosedApproveByUser)
           .WithMany()
           .HasForeignKey(u => u.ClosedApproveBy)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }


}
