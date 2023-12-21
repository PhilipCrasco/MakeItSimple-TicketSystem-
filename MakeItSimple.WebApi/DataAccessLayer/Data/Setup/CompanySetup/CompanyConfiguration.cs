using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.CompanySetup
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
            public void Configure(EntityTypeBuilder<Company> builder)
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
