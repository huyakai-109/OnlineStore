using AutoMapper;
using Training.Api.Models.Requests.Examples;
using Training.Api.Models.Responses.Examples;
using Training.BusinessLogic.Dtos.Examples;
using Training.DataAccess.Entities;

namespace Training.Api.Mappers
{
    public class ExampleMapper : Profile
    {
        public ExampleMapper()
        {
            CreateMap<ExampleNewReq, ExampleDto>();
            CreateMap<ExampleDto, Example>();
            CreateMap<Example, ExampleDto>();
            CreateMap<ExampleDto, ExampleNewRes>();
        }
    }
}
