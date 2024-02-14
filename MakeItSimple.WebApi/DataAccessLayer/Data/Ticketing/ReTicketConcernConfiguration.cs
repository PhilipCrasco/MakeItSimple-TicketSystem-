using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class ReTicketConcernConfiguration : IEntityTypeConfiguration<ReTicketConcern>
    {
        public void Configure(EntityTypeBuilder<ReTicketConcern> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ReTicketByUser)
           .WithMany()
           .HasForeignKey(u => u.ReTicketBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.RejectReTicketByUser)
           .WithMany()
           .HasForeignKey(u => u.RejectReTicketBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
