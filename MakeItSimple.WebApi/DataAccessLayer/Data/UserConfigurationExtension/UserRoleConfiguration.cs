using MakeItSimple.WebApi.Models.UserManagement.UserRoleModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace MakeItSimple.WebApi.DataAccessLayer.Data.UserConfigurationExtension
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasData(new UserRole
            {
                Id = 1,
                UserRoleName = "Admin",
                Permissions = new List<string> { "User Management" },
            });

             builder.Property(e => e.Permissions).HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                new ValueComparer<ICollection<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));


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
