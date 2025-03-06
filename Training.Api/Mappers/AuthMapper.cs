using AutoMapper;
using Training.Api.Models.Requests.Users;
using Training.Api.Models.Responses.Users;
using Training.BusinessLogic.Dtos.Auth;

namespace Training.Api.Mappers
{
    public class AuthMapper : Profile
    {
        public AuthMapper()
        {
            CreateMap<LoginResultDto, LoginRes>();
            CreateMap<LoginReq, LoginReqDto>();
            CreateMap<LogoutReq, LogoutReqDto>();
        }
    }
}
