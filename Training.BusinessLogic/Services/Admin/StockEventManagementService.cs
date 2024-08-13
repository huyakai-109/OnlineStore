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
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services.Admin
{
    public interface IStockEventManagementService
    {
        Task<(List<StockEventDto> StockEvents, Pagination pagination)> GetStockEvents(CommonSearchDto search);
    }
    public class StockEventManagementService(IMapper mapper,
        IUnitOfWork unitOfWork) : IStockEventManagementService
    {
        public async Task<(List<StockEventDto> StockEvents, Pagination pagination)> GetStockEvents(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<StockEvent>().QueryAllWithIncludes(se => !se.Stock.Product.IsDeleted, 
                                                                                                    disableTracking: true, s => s.Stock.Product, s => s.Stock.Product.Category);

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                var searchLower = search.SearchQuery.ToLower();
                query = query.Where(se => se.Stock.Product.Name.Contains(search.SearchQuery)
                                        || se.Stock.Product.Category.Name.Contains(search.SearchQuery));
            }

            var totalCount = await query.CountAsync();
            var stockEvents = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, stockEvents.Count, search.Skip, search.Take);

            return (mapper.Map<List<StockEventDto>>(stockEvents), pagination);
        }
    }
}
