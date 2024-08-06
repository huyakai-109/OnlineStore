using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class UserManagementMapper : Profile
    {
        public UserManagementMapper()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserDto, UserViewModel>().ReverseMap();
            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
