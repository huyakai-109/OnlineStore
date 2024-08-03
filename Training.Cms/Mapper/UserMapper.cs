using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class UserMapper:Profile
    {
        public UserMapper() 
        {
            CreateMap<User, UserDto>();
            CreateMap<LoginVM, UserDto>();
            CreateMap<ChangePasswordViewModel, ChangePasswordDto>();
            CreateMap<UserDto, ProfileViewModel>();
        }
    }
}
