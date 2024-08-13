using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class StockEventManagementMapper : Profile
    {
        public StockEventManagementMapper()
        {
            CreateMap<StockEvent, StockEventDto>()
                .ForMember(sed => sed.Product, o => o.MapFrom(se => se.Stock.Product.Name))
                .ForMember(sed => sed.Category, o => o.MapFrom(se => se.Stock.Product.Category.Name));
            CreateMap<StockEventDto, StockEvent>();
            CreateMap<StockEventDto, StockEventViewModel>();
            CreateMap<StockEventViewModel, StockEventDto>();

            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
