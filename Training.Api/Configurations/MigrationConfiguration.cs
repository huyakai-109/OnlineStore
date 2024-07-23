using Microsoft.EntityFrameworkCore;
using Training.Common.Constants;
using Training.DataAccess.DbContexts;

namespace Training.Api.Configurations
{
    public static class MigrationConfiguration
    {
        public static void RunMigration(this WebApplication app)
        {
            var autoMigration = app.Configuration.GetSection(ConfigKeys.AutoMigration).Get<bool>();
            if (!autoMigration)
            {
                return;
            }

            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            context.Database.Migrate();
        }
    }
}
