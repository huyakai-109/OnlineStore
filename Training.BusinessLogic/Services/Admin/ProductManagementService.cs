using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Common;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;
using static Training.Common.Constants.Permissions;

namespace Training.BusinessLogic.Services.Admin
{
    public interface IProductManagementService
    {
        Task<(List<ProductDto> Products, Pagination Pagination)> GetProducts(CommonSearchDto search);
        Task CreateProduct(ProductDto productDto);
        Task AddImage(ProductImageDto productImageDto);
        Task<List<ProductImageDto>> GetProductImages(long productId);
        Task<List<CategoryDto>> GetCategories();
        Task<ProductDto?> GetProductById(long id);
        Task<bool> UpdateProduct(ProductDto userDto);
        Task UpdateThumbnail(long productId, string thumbnailPath);
        Task DeleteProduct(long Id);
    }
    public class ProductManagementService(IMapper mapper,
        IUnitOfWork unitOfWork) : IProductManagementService
    {
        public async Task<(List<ProductDto> Products, Pagination Pagination)> GetProducts(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<Product>().QueryAllWithIncludes(p => !p.IsDeleted, disableTracking: true, p => p.Category , p => p.CreatedByUser );

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                var searchLower = search.SearchQuery.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower)
                                      || p.Category.Name.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();
            var products = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, products.Count, search.Skip, search.Take);

            return (mapper.Map<List<ProductDto>>(products), pagination);
        }
        public async Task CreateProduct(ProductDto productDto)
        {
            var product = mapper.Map<Product>(productDto);
            
            await unitOfWork.GetRepository<Product>().Add(product);
            await unitOfWork.SaveChanges();

            var stock = new Stock
            {
                ProductId = product.Id,
                Quantity = productDto.StockQuantity
            };
            await unitOfWork.GetRepository<Stock>().Add(stock);

            await unitOfWork.SaveChanges();
        }
        public async Task AddImage(ProductImageDto productImageDto)
        {
            var productImage = mapper.Map<ProductImage>(productImageDto);
            await unitOfWork.GetRepository<ProductImage>().Add(productImage);
            await unitOfWork.SaveChanges();

            // Check if this image should be set as the thumbnail
            if (productImageDto.IsThumbnail && !string.IsNullOrEmpty(productImageDto.Path))
            {
                await UpdateThumbnail(productImageDto.ProductId, productImageDto.Path);
            }
        }
        public async Task UpdateThumbnail(long productId, string thumbnailPath)
        {
            var productRepo = unitOfWork.GetRepository<Product>();
            var product = await productRepo.Single(p => p.Id == productId);
            if (product != null)
            {
                product.Thumbnail = thumbnailPath;
                await productRepo.Update(product);
                await unitOfWork.SaveChanges();
            }
        }

        public async Task<bool> UpdateProduct(ProductDto productDto)
        {
            var productRepo = unitOfWork.GetRepository<Product>();
            var product = await productRepo.Single(u => u.Id == productDto.Id);
            if (product == null)
            {
                return false;
            }

            product.Name = productDto.Name; 
            product.Description = productDto.Description; 
            product.UnitPrice = productDto.UnitPrice; 
            product.CategoryId = product.CategoryId;
            product.Thumbnail = productDto.Thumbnail;

            await productRepo.Update(product);
            await unitOfWork.SaveChanges();

            return true;
        }
        public async Task<List<CategoryDto>> GetCategories()
        {
            var categories = await unitOfWork.GetRepository<Category>().QueryCondition(c => !c.IsDeleted);
            return mapper.Map<List<CategoryDto>>(await categories.ToArrayAsync());
        }
        public async Task DeleteProduct(long Id)
        {
            var productRepo = unitOfWork.GetRepository<Product>();
            var product = await productRepo.Single(u => u.Id == Id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with Id {Id} not found.");
            }
            product.IsDeleted = true;

            await productRepo.Update(product);
            await unitOfWork.SaveChanges();
        }

        public async Task<ProductDto?> GetProductById(long id)
        {
            var product = await unitOfWork.GetRepository<Product>().FindById(id);
            if (product == null) return null;

            var productDto = mapper.Map<ProductDto>(product);
            return productDto;
        }


        public async Task<List<ProductImageDto>> GetProductImages(long productId)
        {
            var images = await unitOfWork.GetRepository<ProductImage>()
                                         .QueryCondition(pi => pi.ProductId == productId);

            var listImage = await images.ToListAsync();

            return mapper.Map<List<ProductImageDto>>(listImage);
        }

    }
}
