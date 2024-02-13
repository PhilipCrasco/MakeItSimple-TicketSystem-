using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.BusinessUnitSetup
{
    public class BusinessUnitConfiguration : IEntityTypeConfiguration<BusinessUnit>
    {
        public void Configure(EntityTypeBuilder<BusinessUnit> builder)
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
