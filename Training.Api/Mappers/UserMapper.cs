using AutoMapper;
using Training.Api.Models.Requests.Users;
using Training.Api.Models.Responses.Users;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;

namespace Training.Api.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<RegisterReq, CustomerDto>();
            CreateMap<CustomerDto, User>().ReverseMap();
            CreateMap<LoginReq, CustomerDto>();
            CreateMap<CustomerDto, ProfileRes>();
            CreateMap<ChangePasswordReq, ChangePasswordDto>();

        }
    }
}
