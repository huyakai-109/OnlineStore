using Minio;
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
            collection.AddMinIO(configuration);
        }

        private static void AddServices(this IServiceCollection collection)
        {
            collection.AddScoped<IAuthService, AuthService>();
            collection.AddScoped<ICustomerProductService, ProductService>();
            collection.AddScoped<ICustomerService, CustomerService>();
            collection.AddScoped<ITokenService, TokenService>();
            collection.AddScoped<ICartService, CartService>();
            collection.AddScoped<IOrderService, OrderService>();
            collection.AddScoped<IPaymentService, PaymentService>();
            collection.AddScoped<IStorageService, StorageService>();
        }

        private static void AddMinIO(this IServiceCollection collection, IConfiguration configuration)
        {
            var endpoint = configuration[ConfigKeys.MinIO.Endpoint];
            var accessKey = configuration[ConfigKeys.MinIO.AccessKey];
            var secrectKey = configuration[ConfigKeys.MinIO.SecretKey];
            var secure = configuration.GetValue<bool>(ConfigKeys.MinIO.Secure);
            var region = configuration[ConfigKeys.MinIO.Region];
            collection.AddMinio(configureClient => configureClient
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secrectKey)
                .WithSSL(secure)
                .WithRegion(region)
                .Build());
        }
    }
}
