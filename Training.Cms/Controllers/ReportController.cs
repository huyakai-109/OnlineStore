using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;
using Training.Common.Constants;

namespace Training.Cms.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ReportController(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        [Authorize(Policy = Permissions.Reports.ViewReports)]
        public async Task<IActionResult> Index(CommonSearchViewModel search)
        {
            var (stocks, pagination) = await _reportService.GetLowStockProducts(_mapper.Map<CommonSearchDto>(search));
            var stockList = new CommonListViewModel<StockViewModel>
            {
                Items = _mapper.Map<List<StockViewModel>>(stocks),
                Pagination = pagination
            };
            return View(stockList);
        }

        [Authorize(Policy = Permissions.Reports.ViewReports)]
        public async Task<IActionResult> TodayOrders(CommonSearchViewModel search)
        {
            var (orders, pagination) = await _reportService.GetTodayOrders(_mapper.Map<CommonSearchDto>(search));

            var orderList = new CommonListViewModel<OrderViewModel>
            {
                Items = _mapper.Map<List<OrderViewModel>>(orders),
                Pagination = pagination 
            };

            return PartialView("_TodaysOrders", orderList);
        }

        [Authorize(Policy = Permissions.Reports.ViewReports)]
        public async Task<IActionResult> HighestOrders(CommonSearchViewModel search)
        {
            var (orders, pagination) = await _reportService.GetHighestOrders(_mapper.Map<CommonSearchDto>(search));

            var orderList = new CommonListViewModel<OrderViewModel>
            {
                Items = _mapper.Map<List<OrderViewModel>>(orders),
                Pagination = pagination
            };

            return PartialView("_HighestOrders", orderList);
        }

    }
}
