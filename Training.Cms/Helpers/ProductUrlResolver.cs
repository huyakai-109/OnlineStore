using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.DataAccess.Entities;

namespace Training.Cms.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly IConfiguration _config;
        public ProductUrlResolver(IConfiguration config)
        {
            _config = config;
        }
        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Thumbnail))
            {
                var apiUrl = _config["ApiUrl"] ?? string.Empty;
                return $"{apiUrl}images/products/{source.Thumbnail}";
            }
            return string.Empty;
        }
    }
}
