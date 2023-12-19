using MakeItSimple.WebApi.DataAccessLayer.Data.UserConfigurationExtension;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace MakeItSimple.WebApi.DataAccessLayer.Data
{
    public class MisDbContext : DbContext
    {
        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

        }


    }
}
