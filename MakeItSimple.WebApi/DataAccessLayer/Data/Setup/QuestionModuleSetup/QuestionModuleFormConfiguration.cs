using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models.Setup.QuestionModuleSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.QuestionModuleSetup
{
    public class QuestionModuleFormConfiguration : IEntityTypeConfiguration<QuestionModuleForm>
    {
        public void Configure(EntityTypeBuilder<QuestionModuleForm> builder)
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
