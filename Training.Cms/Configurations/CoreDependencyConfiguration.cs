using Training.BusinessLogic.Services;
using Training.BusinessLogic.Services.Admin;
using Training.Repository.UoW;

namespace Training.Api.Configurations
{
    public static class CoreDependencyConfiguration
    {
        public static void AddCoreDependencies(this IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddHttpContextAccessor();
            collection.AddServices();
            collection.AddUnitOfWork(configuration);
        }

        private static void AddServices(this IServiceCollection collection)
        {
            collection.AddScoped<IUserService, UserService>();
            collection.AddScoped<ICookieService, CookieService>();
            collection.AddScoped<IUserManagementService, UserManagementService>();
            collection.AddScoped<ICategoryManagementService, CategoryManagementService>();
            collection.AddScoped<IProductManagementService, ProductManagementService>();
            collection.AddScoped<IStockManagementService, StockManagementService>();
            collection.AddScoped<IStockEventManagementService, StockEventManagementService>();
            
        }
    }
}
