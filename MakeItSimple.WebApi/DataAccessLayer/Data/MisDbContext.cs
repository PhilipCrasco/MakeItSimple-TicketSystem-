using MakeItSimple.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Data
{
    public class MisDbContext : DbContext
    {
        public MisDbContext(DbContextOptions<MisDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = Guid.NewGuid(),
                Fullname = "Crasco , Philip Lorenz R.",
                Username = "pcrasco",
                Password = "$2a$12$SxkZHwqc0st2uk2fxsnQduvfm6GZxJXymbcsueo4Fnw1wKSmNDcs2"

            });
        }


    }
}
