﻿using AutoMapper;
using Training.Api.Models.Requests.Carts;
using Training.Api.Models.Responses.Cart;
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

            CreateMap<Cart, CartDto>();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(ci => ci.ProductName, o => o.MapFrom(ci => ci.Product.Name))
                .ForMember(ci => ci.Thumbnail, o => o.MapFrom(ci => ci.Product.Thumbnail));

            CreateMap<CartDto, CartRes>();  
            CreateMap<CartItemDto, CartItemRes>();  
        }
    }
}
