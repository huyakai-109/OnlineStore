using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.DataAccess.Entities;

namespace Training.Cms.Helpers
{
    public class CategoryUrlResolver : IValueResolver<Category, CategoryDto, string?>
    {

        private readonly IConfiguration _config;
        public CategoryUrlResolver(IConfiguration config)
        {
            _config = config;
        }
        public string Resolve(Category source, CategoryDto destination, string? destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Image))
            {
                var apiUrl = _config["ApiUrl"] ?? string.Empty;
                return $"{apiUrl}/{source.Image}";
            }
            return string.Empty;
        }
    }
}
