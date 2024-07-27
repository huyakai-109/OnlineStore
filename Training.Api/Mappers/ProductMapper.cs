using AutoMapper;
using Training.Api.Models.Responses.Products;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;

namespace Training.Api.Mappers
{
    public class ProductMapper:Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, CustomerProductDto>()
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.Name))
                .ReverseMap();
            CreateMap<CustomerProductDto, ProductRes>();
                

        }
    }
}
