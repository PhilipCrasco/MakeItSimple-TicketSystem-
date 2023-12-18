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
