using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.SubUnitSetup
{
    public class SubUnitConfiguration : IEntityTypeConfiguration<SubUnit>
    {
        public void Configure(EntityTypeBuilder<SubUnit> builder)
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
