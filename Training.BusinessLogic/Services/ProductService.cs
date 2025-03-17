using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Dtos.Customers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;
using static Training.Common.Constants.GlobalConstants;

namespace Training.BusinessLogic.Services
{

    public interface ICustomerProductService
    {
        Task<(List<CustomerProductDto> Items, int TotalCount, int CurrentCount)> GetProducts(CommonSearchDto search);

        Task<CustomerProductDto?> GetProductByIdAsync(long id);

        Task<bool> AddToCartAsync(AddToCartDto addToCartDto);
    }

    public class ProductService(
        IMapper mapper,
        IUnitOfWork unitOfWork) : ICustomerProductService
    {

        public async Task<CustomerProductDto?> GetProductByIdAsync(long id)
        {
            var product = await (from pr in await unitOfWork.GetRepository<Product>().QueryAll()
                                 join cat in await unitOfWork.GetRepository<Category>().QueryAll()
                                 on pr.CategoryId equals cat.Id
                                 where pr.Id == id
                                 select new CustomerProductDto()
                                 {
                                     Id = pr.Id,
                                     Name = pr.Name,
                                     Description = pr.Description,
                                     UnitPrice = pr.UnitPrice,
                                     Category = cat.Name,
                                     Thumbnail = pr.Thumbnail,
                                 }).FirstOrDefaultAsync();

            if (product == null) return null;

            var productImages = await unitOfWork.GetRepository<ProductImage>().QueryCondition(pi => pi.ProductId == id);
            if (productImages == null) return null;

            var productDto = mapper.Map<CustomerProductDto>(product);
            var listImage = await productImages.ToListAsync();
            productDto.ProductImage = productImages.Where(pi => !string.IsNullOrEmpty(pi.Path))
                                                   .Select(pi => pi.Path)
                                                   .ToList();

            return productDto;
        }

        public async Task<(List<CustomerProductDto> Items, int TotalCount, int CurrentCount)> GetProducts(CommonSearchDto search)
        {
            var query = from pr in await unitOfWork.GetRepository<Product>().QueryAll()
                        join cat in await unitOfWork.GetRepository<Category>().QueryAll()
                        on pr.CategoryId equals cat.Id
                        select new CustomerProductDto()
                        {
                            Id = pr.Id,
                            Name = pr.Name,
                            Description = pr.Description,
                            UnitPrice = pr.UnitPrice,
                            Category = cat.Name,
                            Thumbnail = pr.Thumbnail,
                        };

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                var searchLower = search.SearchQuery.ToLower();

                query = query.Where(p => p.Category!.ToLower().Contains(searchLower)
                                        || p.Name!.ToLower().Contains(searchLower));
            }
            if (search.Sort == SortDirection.Ascending)
            {
                query = query.OrderBy(p => p.UnitPrice);
            }
            else if (search.Sort == SortDirection.Descending)
            {
                query = query.OrderByDescending(p => p.UnitPrice);
            }

            var totalCount = await query.CountAsync();
            var products = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();
            var currentCount = products.Count;

            return (mapper.Map<List<CustomerProductDto>>(products), totalCount, currentCount);
        }

        public async Task<bool> AddToCartAsync(AddToCartDto addToCartDto)
        {
            var cartRepo = unitOfWork.GetRepository<Cart>();
            var cartItemRepo = unitOfWork.GetRepository<CartItem>();
            var stockRepo = unitOfWork.GetRepository<Stock>();

            // get or create cart for the user
            var cart = await cartRepo.Single(c => c.UserId == addToCartDto.UserId && !c.IsPurchased);
            if (cart == null)
            {
                var random = new Random();

                cart = new Cart()
                {
                    Id = random.NextInt64(1, long.MaxValue),
                    UserId = addToCartDto.UserId,
                    IsPurchased = false,
                };

                await cartRepo.Add(cart);
            }

            // add  cart item
            var cartItem = await cartItemRepo.Single(i => i.ProductId == addToCartDto.ProductId && i.CartId == cart.Id);

            var stock = await stockRepo.Single(s => s.ProductId == addToCartDto.ProductId);
            if (stock == null || stock.Quantity < addToCartDto.Quantity + (cartItem?.Quantity ?? 0))
            {
                throw new InvalidOperationException("Insufficient stock quantity.");
            }

            if (cartItem == null)
            {
                cartItem = new CartItem()
                {
                    CartId = cart.Id,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                };

                await cartItemRepo.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += addToCartDto.Quantity;
                await cartItemRepo.Update(cartItem);
            }

            await unitOfWork.SaveChanges();
            return true;
        }
    }
}
