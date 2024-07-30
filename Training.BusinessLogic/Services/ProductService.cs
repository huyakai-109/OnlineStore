using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Dtos;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;


namespace Training.BusinessLogic.Services
{
   
     public interface ICustomerProductService
     {
            Task<(List<CustomerProductDto> Items, int TotalCount, int CurrentCount)> GetProducts(SearchDto searchDto);
            Task<(List<CustomerProductDto> Items, int TotalCount, int CurrentCount)> SearchProducts(ProductSearchDto searchDto);
            Task<CustomerProductDto> GetProductByIdAsync(long id);
           // Task<bool> AddToCartAsync(CustomerCartItemDto cartItemDto);
     }
    
    public class ProductService(
        IMapper mapper,
        IUnitOfWork unitOfWork) : ICustomerProductService
    {
   
        public async Task<CustomerProductDto> GetProductByIdAsync(long id)
        {
            var product = await unitOfWork.GetRepository<Product>().Single(p => p.Id == id, include: p => p.Include(x => x.Category));
            if (product == null) return null;

            return mapper.Map<CustomerProductDto>(product);
        }

        public async Task<(List<CustomerProductDto> Items, int TotalCount, int CurrentCount)> GetProducts(SearchDto searchDto)
        {
            var query = await unitOfWork.GetRepository<Product>().QueryAllWithIncludes(disableTracking: true,p => p.Category);
            var totalCount = await query.CountAsync();
            var products = await query.Skip(searchDto.Skip).Take(searchDto.Take).ToListAsync();
            var currentCount = products.Count;

            return (mapper.Map<List<CustomerProductDto>>(products), totalCount, currentCount);
        }

        public async Task<(List<CustomerProductDto> Items, int TotalCount, int CurrentCount)> SearchProducts(ProductSearchDto searchDto)
        {
            var query = await unitOfWork.GetRepository<Product>().QueryAllWithIncludes(disableTracking: true,p => p.Category);

            if (!string.IsNullOrEmpty(searchDto.Category))
            {
                query = query.Where(p => p.Category.Name.ToLower().Contains(searchDto.Category.ToLower())); 
            }

            if (!string.IsNullOrEmpty(searchDto.Name))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchDto.Name.ToLower()));
            }

            query = searchDto.SortByPriceAscending ? query.OrderBy(p => p.UnitPrice) : query.OrderByDescending(p => p.UnitPrice);

            var totalCount = await query.CountAsync();
            var products = await query.Skip(searchDto.Skip).Take(searchDto.Take).ToListAsync();
            var currentCount = products.Count;

            return (mapper.Map<List<CustomerProductDto>>(products), totalCount, currentCount);
        }
    }
}
