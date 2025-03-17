using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models.Requests.Orders;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Orders;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Services;
using Training.Common.Helpers;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController(ILogger<OrdersController> logger,
        IMapper mapper,
        IOrderService orderService): BaseController(logger, mapper)
    {
        [HttpPost("purchase")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> PurchaseCart([FromBody] PurchaseCartReq purchaseCartReq)
        {
            var response = new ResultRes<bool>();

            try
            {
                var userId = this.User.Claims.GetUserId();
                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    response.Error = "User ID not found";
                    return Unauthorized(response);
                }

                var purchaseCartDto = Mapper.Map<PurchaseCartDto>(purchaseCartReq);
                purchaseCartDto.UserId = userId;

                var result = await orderService.PurchaseCart(purchaseCartDto);

                if (!result)
                {
                    response.Error = "Failed to purchase cart";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Result = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Purchase cart failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultRes<List<OrderRes>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            var response = new ResultRes<List<OrderRes>>();

            try
            {
                var userId = this.User.Claims.GetUserId();
                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    response.Error = "User ID not found";
                    return Unauthorized(response);
                }

                var orders = await orderService.GetOrders(userId);

                response.Success = true;
                response.Result = Mapper.Map<List<OrderRes>>(orders);
                return Ok(response);

            }
            catch (Exception ex)
            {
                Logger.LogError("Get orders failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
    }
}
