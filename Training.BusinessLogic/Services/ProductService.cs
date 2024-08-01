using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
            Task<CustomerProductDto?> GetProductByIdAsync(long id);
            Task<bool> AddToCartAsync(AddToCartDto addToCartDto);
    }
    
    public class ProductService(
        IMapper mapper,
        IUnitOfWork unitOfWork) : ICustomerProductService
    {
   
        public async Task<CustomerProductDto?> GetProductByIdAsync(long id)
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

            query = searchDto.Ascending ? query.OrderBy(p => p.UnitPrice) : query.OrderByDescending(p => p.UnitPrice);

            var totalCount = await query.CountAsync();
            var products = await query.Skip(searchDto.Skip).Take(searchDto.Take).ToListAsync();
            var currentCount = products.Count;

            return (mapper.Map<List<CustomerProductDto>>(products), totalCount, currentCount);
        }
        public async Task<bool> AddToCartAsync(AddToCartDto addToCartDto)
        {
            using var transaction = await unitOfWork.BeginTransactionAsync();

            try
            {
                var cartRepo = unitOfWork.GetRepository<Cart>();
                var cartItemRepo = unitOfWork.GetRepository<CartItem>();
                var stockRepo = unitOfWork.GetRepository<Stock>();

                var stock = await stockRepo.Single(s => s.ProductId == addToCartDto.ProductId);
                if (stock == null || stock.Quantity < addToCartDto.Quantity)
                {
                    throw new InvalidOperationException("Insufficient stock quantity.");
                }

                // get or create cart for the user
                var cart = await cartRepo.Single(c => c.UserId == addToCartDto.UserId && !c.IsPurchased, include: c => c.Include(c => c.CartItems));
                if (cart == null)
                {
                    cart = mapper.Map<Cart>(addToCartDto);
                    cart.IsPurchased = false;

                    await cartRepo.Add(cart);
                    await unitOfWork.SaveChanges();
                }

                // add  cart item
                var cartItem = mapper.Map<CartItem>(addToCartDto);
                cartItem.CartId = cart.Id;
                cart.CartItems.Add(cartItem);
                await cartItemRepo.Add(cartItem);
               
                await unitOfWork.SaveChanges();
                await transaction.CommitAsync(); 
                return true;
            }
            catch (Exception ) {
                await transaction.RollbackAsync();
                throw;
            }

        }
    }
}
