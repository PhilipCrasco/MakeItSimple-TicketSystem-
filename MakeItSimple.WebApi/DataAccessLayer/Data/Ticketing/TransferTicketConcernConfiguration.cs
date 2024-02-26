using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TransferTicketConcernConfiguration : IEntityTypeConfiguration<TransferTicketConcern>
    {
        public void Configure(EntityTypeBuilder<TransferTicketConcern> builder)
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

            builder.HasOne(u => u.RejectTransferByUser)
           .WithMany()
           .HasForeignKey(u => u.RejectTransferBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }

}
