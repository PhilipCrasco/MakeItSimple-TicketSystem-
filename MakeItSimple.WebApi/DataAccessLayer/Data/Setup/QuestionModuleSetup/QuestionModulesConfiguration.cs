using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using MakeItSimple.WebApi.Models.Setup.QuestionModuleSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.Setup.QuestionModuleSetup
{
    public class QuestionModulesConfiguration : IEntityTypeConfiguration<QuestionModule>
    {
        public void Configure(EntityTypeBuilder<QuestionModule> builder)
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
