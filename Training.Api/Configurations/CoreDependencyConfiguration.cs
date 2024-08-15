using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Training.BusinessLogic.Services;
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
            collection.AddJwtAuthentication(configuration);
        }

        private static void AddServices(this IServiceCollection collection)
        {
            collection.AddScoped<IExampleService, ExampleService>();
            collection.AddScoped<ICustomerProductService, ProductService>();
            collection.AddScoped<ICustomerService, CustomerService>();
            collection.AddScoped<ITokenService, TokenService>();
            collection.AddScoped<ICartService, CartService>();
            collection.AddScoped<IOrderService, OrderService>();
        }

        private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var secretKey = config[ConfigKeys.Security.Jwt.Secret];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config[ConfigKeys.Security.Jwt.Issuer],
                        ValidAudience = config[ConfigKeys.Security.Jwt.Audience],
                        IssuerSigningKey = key
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var authorization = context.Request.Headers[RestfulConstants.RequestHeaders.Authorization].FirstOrDefault();
                            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith(RestfulConstants.RequestHeaders.AuthScheme))
                            {
                                context.Token = authorization.Substring(RestfulConstants.RequestHeaders.AuthScheme.Length).Trim();
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.MapInboundClaims = false;
                });
        }
    }
}
