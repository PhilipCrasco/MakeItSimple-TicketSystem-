using MakeItSimple.WebApi.Models.Setup.CategorySetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.ApproverSetup
{
    public class ApproverConfiguration : IEntityTypeConfiguration<Approver>
    {
        public void Configure(EntityTypeBuilder<Approver> builder)
        {

            builder.HasOne(u => u.AddedByUser)
           .WithMany()
           .HasForeignKey(u => u.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(u => u.ModifiedByUser)
           .WithMany()
           .HasForeignKey(u => u.ModifiedBy)
           .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
