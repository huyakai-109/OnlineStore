using Microsoft.AspNetCore.Authentication.Cookies;
using Training.Common.Constants;
using Training.DataAccess.DbContexts;
using Training.DataAccess.Entities;

namespace Training.Cms.Configurations
{
    public static class SecurityConfiguration
    {
        public static void AddSecurity(this IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddDefaultIdentity<User>(options =>
                {
                    options.Lockout.MaxFailedAccessAttempts = configuration.GetSection(ConfigKeys.Security.Lockout.MaxFailedAccessAttempts).Get<int>();
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetSection(ConfigKeys.Security.Lockout.DefaultLockoutMinutes).Get<int>());
                })
                .AddEntityFrameworkStores<MyDbContext>();

            collection.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = "/Account/Login";
                       options.AccessDeniedPath = "/Account/AccessDenied";
                       options.Cookie.HttpOnly = true;
                   });

            collection.AddAuthorization(options =>
            {
                foreach (var permission in Permissions.All)
                {
                    options.AddPolicy(permission, policy =>
                        policy.RequireClaim(RolePolicies.ClaimType, permission));
                }
            });
        }
    }
}
