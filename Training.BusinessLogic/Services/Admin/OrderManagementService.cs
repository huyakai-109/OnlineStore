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
    public interface IOrderManagementService
    {
        Task<(List<OrderDto> Orders, Pagination pagination)> GetOrders(CommonSearchDto search);
    }
    public class OrderManagementService(IMapper mapper,
        IUnitOfWork unitOfWork) : IOrderManagementService
    {
        public async Task<(List<OrderDto> Orders, Pagination pagination)> GetOrders(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<Order>().QueryAllWithIncludes(null, disableTracking: true, o => o.Customer);

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                query = query.Where(o => o.Customer.FirstName.Contains(search.SearchQuery)
                                        || o.Customer.LastName.Contains(search.SearchQuery));
            }

            var totalCount = await query.CountAsync();
            var orders = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, orders.Count, search.Skip, search.Take);

            return (mapper.Map<List<OrderDto>>(orders), pagination);
        }
    }
}
