using AutoMapper;
using Training.Api.Models.Requests.Carts;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;

namespace Training.Api.Mappers
{
    public class CartMapper : Profile
    {
        public CartMapper()
        {
            CreateMap<AddToCartReq, AddToCartDto>();
            CreateMap<AddToCartDto, Cart>();
            CreateMap<AddToCartDto, CartItem>();
            CreateMap<EditQuantityReq, EditCartQuantityDto>();
            CreateMap<RemoveProductFCartReq, RemoveProductFCartDto>();
        }
    }
}
