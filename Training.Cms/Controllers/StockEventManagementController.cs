using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;

namespace Training.Cms.Controllers
{
    [Authorize(Policy = "AdminOrClerk")]
    public class StockEventManagementController : Controller
    {
        private readonly IStockEventManagementService _stockEventManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<StockEventManagementController> _logger;

        public StockEventManagementController(IStockEventManagementService stockEventManagementService, 
            IMapper mapper, ILogger<StockEventManagementController> logger)
        {
            _stockEventManagementService = stockEventManagementService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CommonSearchViewModel search)
        {
            var (stockEvents, pagination) = await _stockEventManagementService.GetStockEvents(_mapper.Map<CommonSearchDto>(search));

            var listStockEvent = new CommonListViewModel<StockEventViewModel>
            {
                Items = _mapper.Map<List<StockEventViewModel>>(stockEvents),
                Pagination = pagination
            };
            return View(listStockEvent);
        }
    }
}
