using AutoMapper;
using Training.Api.Models.Requests.Orders;
using Training.Api.Models.Responses.Orders;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;

namespace Training.Api.Mappers
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<PurchaseCartReq, PurchaseCartDto>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, OrderRes>();
            CreateMap<OrderDetail, OrderDetailDTO>()
                    .ForMember(odd => odd.ProductName, o => o.MapFrom(o => o.Product.Name))
                    .ForMember(odd => odd.Thumbnail, o => o.MapFrom(o => o.Product.Thumbnail));
            CreateMap<OrderDetailDTO, OrderDetailRes>();
        }
    }
}
