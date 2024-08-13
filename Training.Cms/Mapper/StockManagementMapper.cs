using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class StockManagementMapper : Profile
    {
        public StockManagementMapper()
        {
            CreateMap<Stock, StockDto>()
                .ForMember(sd => sd.Product, o => o.MapFrom(s => s.Product.Name))
                .ForMember(sd => sd.Category, o => o.MapFrom(s => s.Product.Category.Name));
            CreateMap<StockDto, Stock>();
            CreateMap<StockDto, StockViewModel>();

            CreateMap<StockEvent, StockEventDto>();
            CreateMap<StockEventDto, StockEvent>();
            CreateMap<StockEventDto, StockEventViewModel>();
            CreateMap<StockEventViewModel, StockEventDto>();

            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
