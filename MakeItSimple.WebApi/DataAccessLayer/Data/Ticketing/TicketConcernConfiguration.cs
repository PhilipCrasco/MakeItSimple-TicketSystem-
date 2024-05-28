using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Ticketing;

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

            builder.HasOne(u => u.TransferByUser)
           .WithMany()
           .HasForeignKey(u => u.TransferBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ApprovedByUser)
           .WithMany()
           .HasForeignKey(u => u.ApprovedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ClosedApproveByUser)
           .WithMany()
           .HasForeignKey(u => u.ClosedApproveBy)
           .OnDelete(DeleteBehavior.Restrict);

           builder.HasOne(u => u.ReticketByUser)
           .WithMany()
           .HasForeignKey(u => u.ReticketBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.RequestorByUser)
           .WithMany()
           .HasForeignKey(u => u.RequestorBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ReDateByUser)
           .WithMany()
           .HasForeignKey(u => u.ReDateBy)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }


}
