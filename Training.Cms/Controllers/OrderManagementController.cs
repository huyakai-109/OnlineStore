using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Services.Admin;
using Training.Cms.Models;

namespace Training.Cms.Controllers
{
    [Authorize(Policy = "AdminOrClerk")]
    public class OrderManagementController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IMapper _mapper;

        public OrderManagementController(IOrderManagementService orderManagementService, IMapper mapper)
        {
            _orderManagementService = orderManagementService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(CommonSearchViewModel search)
        {
            var (orders, pagination) = await _orderManagementService.GetOrders(_mapper.Map<CommonSearchDto>(search));
            var orderList = new CommonListViewModel<OrderViewModel>
            {
                Items = _mapper.Map<List<OrderViewModel>>(orders),
                Pagination = pagination
            };
            return View(orderList);
        }
    }
}
