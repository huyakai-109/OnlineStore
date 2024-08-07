using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class CategoryManagementMapper: Profile
    {
        public CategoryManagementMapper() 
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryDto, CategoryViewModel>().ReverseMap();
            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
