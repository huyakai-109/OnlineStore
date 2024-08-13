using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Common;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services.Admin
{
    public interface IStockManagementService
    {
        Task<(List<StockDto> Stocks, Pagination  pagination)>GetStocks(CommonSearchDto search);
        Task AdjustStock(StockEventDto stockEventDto);
    }
    public class StockManagementService(IMapper mapper,
        IUnitOfWork unitOfWork) : IStockManagementService
    {

        public async Task<(List<StockDto> Stocks, Pagination pagination)> GetStocks(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<Stock>().QueryAllWithIncludes(s => !s.Product.IsDeleted ,disableTracking: true, s => s.Product, s => s.Product.Category);

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                var searchLower = search.SearchQuery.ToLower();
                query = query.Where(s => s.Product.Name.Contains(searchLower)
                                       || s.Product.Category.Name.Contains(searchLower));
            }

            var totalCount = await query.CountAsync();
            var stocks = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, stocks.Count, search.Skip, search.Take);

            return (mapper.Map<List<StockDto>>(stocks), pagination);    
        }
        public async Task AdjustStock(StockEventDto stockEventDto)
        {
            var stockRepo = unitOfWork.GetRepository<Stock>();
            var stock = await stockRepo.Single(s => s.Id == stockEventDto.StockId);

            if (stock == null) throw new Exception("Stock not found");
            if(stockEventDto.Type == Training.Common.EnumTypes.StockEventType.In)
            {
                stock.Quantity += stockEventDto.Quantity;
            }
            else
            {
                if (stock.Quantity < stockEventDto.Quantity)
                {
                    throw new Exception("Insufficient stock");
                }
                stock.Quantity -= stockEventDto.Quantity;
            }
            var stockEvent = mapper.Map<StockEvent>(stockEventDto);
            stockEvent.CreatedAt = DateTimeOffset.UtcNow;

            await unitOfWork.GetRepository<StockEvent>().Add(stockEvent);
            await stockRepo.Update(stock);

            await unitOfWork.SaveChanges();
        }
    }
}
