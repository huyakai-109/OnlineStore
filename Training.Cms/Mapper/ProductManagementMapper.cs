using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Helpers;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class ProductManagementMapper : Profile
    {
        public ProductManagementMapper()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(pd => pd.Category, o => o.MapFrom(p => p.Category.Name))
                .ForMember(pd => pd.CreatedBy, o => o.MapFrom(p => (p.CreatedByUser.FirstName + " " + p.CreatedByUser.LastName)))
                .ForMember(d => d.Thumbnail, o => o.MapFrom<ProductUrlResolver>());

            CreateMap<ProductDto, Product>();
            CreateMap<ProductDto, ProductViewModel>().ReverseMap();

            CreateMap<ProductImageViewModel, ProductImageDto>();
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<ProductImageDto, ProductImage>();

            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
