using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;
using Training.Common.EnumTypes;

namespace Training.Cms.Controllers
{
    [Authorize(Policy = "AdminOrClerk")]
    public class StockManagementController : Controller
    {
        private readonly IStockManagementService _stockManagementService;
        private readonly IMapper _mapper;
        private readonly ILogger<StockManagementController> _logger;

        public StockManagementController(IStockManagementService stockManagementService, IMapper mapper, ILogger<StockManagementController> logger)
        {
            _stockManagementService = stockManagementService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CommonSearchViewModel search)
        {
            var (stocks, pagination) = await _stockManagementService.GetStocks(_mapper.Map<CommonSearchDto>(search));

            var stockList = new CommonListViewModel<StockViewModel>
            {
                Items = _mapper.Map<List<StockViewModel>>(stocks),
                Pagination = pagination
            };

            return View(stockList);
        }
        public IActionResult Edit(long id)
        {
            var model = new StockEventViewModel
            {
                StockId = id
            };
            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StockEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = "Invalid data provided. Please check and try again.";
                return RedirectToAction("Error", new { message = errorMessage, controllerName = "StockManagement" });
            }

            try
            {
                if (model.Type == StockEventType.In)
                {
                    await _stockManagementService.AdjustStock(_mapper.Map<StockEventDto>(model));
                }
                else if (model.Type == StockEventType.Out)
                {
                    await _stockManagementService.AdjustStock(_mapper.Map<StockEventDto>(model));
                }
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                var errorMessage = "Unable to save changes. The user was updated by another user.";
                return RedirectToAction("Error", new { message = errorMessage, controllerName = "StockManagement" });
            }
            catch (Exception ex)
            {
                var errorMessage = $"{ex.Message}";
                return RedirectToAction("Error", new { message = errorMessage, controllerName = "StockManagement" });
            }
          
        }
        public IActionResult Error(string message, string controllerName)
        {
            ViewData["ErrorMessage"] = message;
            ViewData["ControllerName"] = controllerName;
            return View();
        }
    }
}
