using MakeItSimple.WebApi.DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.Common
{
    public static class MigrationExtention
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using MisDbContext dbContext = scope.ServiceProvider.GetRequiredService<MisDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
