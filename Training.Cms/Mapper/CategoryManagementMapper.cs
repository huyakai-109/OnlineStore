using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Helpers;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class CategoryManagementMapper: Profile
    {
        public CategoryManagementMapper() 
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(cd => cd.ImagePath, o => o.MapFrom<CategoryUrlResolver>());
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImagePath));

            CreateMap<CategoryDto, CategoryViewModel>().ReverseMap();
            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
