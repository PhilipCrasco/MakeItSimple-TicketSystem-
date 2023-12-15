using MakeItSimple.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.UserConfigurationExtension
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasData(new User
            {
                Id = Guid.NewGuid(),
                Fullname = "Admin",
                Username = "admin",
                Email = "admin@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                UserRoleId = 1
            });


            builder.HasOne(x => x.AddedByUser)
           .WithMany()
           .HasForeignKey(x => x.AddedBy)
           .OnDelete(DeleteBehavior.Restrict);
           
            
            builder.HasOne(x => x.ModifiedByUser)
           .WithMany()
           .HasForeignKey(x => x.ModifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        }



    }
}
