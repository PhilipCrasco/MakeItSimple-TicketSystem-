using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Ticketing.TicketRequest;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class TicketTransactionConfiguration : IEntityTypeConfiguration<TicketTransaction>
    {
        public void Configure(EntityTypeBuilder<TicketTransaction> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ApprovedByUser)
           .WithMany()
           .HasForeignKey(u => u.ApprovedBy)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
