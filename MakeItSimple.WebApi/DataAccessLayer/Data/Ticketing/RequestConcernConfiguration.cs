using MakeItSimple.WebApi.Models.Ticketing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Ticketing
{
    public class RequestConcernConfiguration : IEntityTypeConfiguration<RequestConcern>
    {
        public void Configure(EntityTypeBuilder<RequestConcern> builder)
        {


            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.RejectByUser)
           .WithMany()
           .HasForeignKey(u => u.RejectBy)
           .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
