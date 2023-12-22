using MakeItSimple.WebApi.Models.Setup.AccountTitleSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.AccountTitleSetup
{
    public class AccountTitleConfiguration : IEntityTypeConfiguration<AccountTitle>
    {
        public void Configure(EntityTypeBuilder<AccountTitle> builder)
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
