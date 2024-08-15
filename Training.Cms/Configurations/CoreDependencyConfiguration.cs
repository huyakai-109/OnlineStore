using Microsoft.AspNetCore.Authentication.Cookies;
using Training.BusinessLogic.Services;
using Training.BusinessLogic.Services.Admin;
using Training.Common.Constants;
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
            collection.AddAuthenticationAndAuthorization();
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
            collection.AddScoped<IOrderManagementService, OrderManagementService>();
            collection.AddScoped<IReportService, ReportService>();
            
        }

        private static void AddAuthenticationAndAuthorization(this IServiceCollection collection)
        {
            collection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.Cookie.HttpOnly = true;
                });

            collection.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireClaim(RolePolicies.ClaimType, RolePolicies.SysAdmin.Name));

                options.AddPolicy("AdminOrClerk", policy =>
                    policy.RequireClaim(RolePolicies.ClaimType, RolePolicies.SysAdmin.Name, RolePolicies.Clerk.Name));
            });
        }
    }
}
