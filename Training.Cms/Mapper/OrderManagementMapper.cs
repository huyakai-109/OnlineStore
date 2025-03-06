using AutoMapper;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Cms.Models;
using Training.DataAccess.Entities;

namespace Training.Cms.Mapper
{
    public class OrderManagementMapper : Profile
    {
        public OrderManagementMapper()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();

            CreateMap<OrderDto, OrderViewModel>();
            CreateMap<OrderViewModel, OrderDto>();

            CreateMap<CommonSearchViewModel, CommonSearchDto>();
        }
    }
}
