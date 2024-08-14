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
    public interface IReportService
    {
        Task<(List<StockDto> Stocks, Pagination pagination)> GetLowStockProducts(CommonSearchDto search);
        Task<(List<OrderDto> Orders, Pagination pagination)> GetTodayOrders(CommonSearchDto search);
        Task<(List<OrderDto> Orders, Pagination pagination)> GetHighestOrders(CommonSearchDto search);
    }
    public class ReportService(IMapper mapper,
        IUnitOfWork unitOfWork) : IReportService
    {
        

        public async Task<(List<StockDto> Stocks, Pagination pagination)> GetLowStockProducts(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<Stock>().QueryAllWithIncludes(s => s.Quantity < 10, disableTracking: true, s => s.Product);

            var totalCount = await query.CountAsync();
            var products = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, products.Count, search.Skip, search.Take);
            
            return (mapper.Map<List<StockDto>>(products), pagination);
        }

        public async Task<(List<OrderDto> Orders, Pagination pagination)> GetTodayOrders(CommonSearchDto search)
        {
            var now = DateTimeOffset.UtcNow;
            var startOfDayUtc = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero);
            var endOfDayUtc = startOfDayUtc.AddDays(1);

            var query = await unitOfWork.GetRepository<Order>()
                                  .QueryAllWithIncludes(o => o.CreatedAt >= startOfDayUtc && o.CreatedAt < endOfDayUtc, disableTracking:true, o => o.Customer);

            var totalCount = await query.CountAsync();  
            var orders = await query.Skip((search.Skip -1)* search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, orders.Count, search.Skip, search.Take);

            return (mapper.Map<List<OrderDto>>(orders), pagination);

        }

        public async Task<(List<OrderDto> Orders, Pagination pagination)> GetHighestOrders(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<Order>().QueryAllWithIncludes(null, disableTracking: true, o => o.Customer); 
            var orders = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();
            var totalCount = await query.CountAsync();
            var pagination = new Pagination(totalCount, orders.Count, search.Skip, search.Take);

            List<OrderDto> ordersDto = new List<OrderDto>();
            mapper.Map(orders, ordersDto);

            foreach (var order in ordersDto) 
            {
                var orderDetailRepo = await unitOfWork.GetRepository<OrderDetail>().QueryCondition(od => od.OrderId == order.Id);
                var orderDetais = await orderDetailRepo.ToListAsync();

                order.TotalPrice = orderDetais.Sum(od => od.UnitPrice * od.Quantity);
            }
    
            return (ordersDto.OrderByDescending(o => o.TotalPrice).ToList(), pagination);
        }
    }
}
