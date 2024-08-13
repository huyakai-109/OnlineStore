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
using Training.Common.EnumTypes;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services.Admin
{
    public interface ICategoryManagementService
    {
        Task<(List<CategoryDto> Categories, Pagination Pagination)> GetCategories(CommonSearchDto search);
        Task CreateCategory(CategoryDto categoryDto);
        Task<CategoryDto?> GetCategoryById(long id);

        Task<bool> UpdateCategory(CategoryDto userDto);
        Task DeleteCategory(long Id);
    }
    public class CategoryManagementService(IMapper mapper,
        IUnitOfWork unitOfWork) : ICategoryManagementService
    {
        public async Task CreateCategory(CategoryDto categoryDto)
        {
            var category = mapper.Map<Category>(categoryDto);

            await unitOfWork.GetRepository<Category>().Add(category);
            await unitOfWork.SaveChanges();
        }

        public async Task DeleteCategory(long Id)
        {
            var categoryRepo = unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.Single(u => u.Id == Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Product with Id {Id} not found.");
            }
            category.IsDeleted = true;
            // delete products corresponding with category
            var productRepo =  unitOfWork.GetRepository<Product>();
            var products = await productRepo.QueryCondition(p => p.CategoryId == Id);
            var listProduct = await products.ToListAsync();
            foreach (var item in listProduct)
            {
                item.IsDeleted = true; 
                await productRepo.Update(item);
            }

            await categoryRepo.Update(category);
            await unitOfWork.SaveChanges();
        }

        public async Task<(List<CategoryDto> Categories, Pagination Pagination)> GetCategories(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<Category>().QueryCondition(u => !u.IsDeleted);

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                var searchLower = search.SearchQuery.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(searchLower));
            }

            var totalCount = await query.CountAsync();
            var categories = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, categories.Count, search.Skip, search.Take);

            return (mapper.Map<List<CategoryDto>>(categories), pagination);
        }

        public async Task<CategoryDto?> GetCategoryById(long id)
        {
            var category = await unitOfWork.GetRepository<Category>().FindById(id);
            if (category == null) return null;

            return mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> UpdateCategory(CategoryDto categoryDto)
        {
            var categoryRepo = unitOfWork.GetRepository<Category>();
            var category = await categoryRepo.Single(u => u.Id == categoryDto.Id);
            if (category == null)
            {
                return false;
            }

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            if (!string.IsNullOrEmpty(categoryDto.ImagePath))
            {
                category.Image = categoryDto.ImagePath;
            }
            await categoryRepo.Update(category);
            await unitOfWork.SaveChanges();

            return true;

        }
    }
}
