using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Training.Common.Constants;
using Training.DataAccess.DbContexts;

namespace Training.Repository.UoW
{
    public static class UnitOfWorkInjection
    {
        public static void AddUnitOfWork(this IServiceCollection collection, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection(ConfigKeys.DatabaseConnection).Value;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "Database connection string is null");
            }

            var serverVersion = new MySqlServerVersion(new Version(9, 0, 0));

            collection.AddDbContext<MyDbContext>(options =>
                options.UseMySql(connectionString, serverVersion));

            collection.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
