using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Training.Api.Models.Requests.Orders;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Orders;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Services;
using Training.DataAccess.Entities;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController(ILogger<OrdersController> logger,
        IMapper mapper,
        IOrderService orderService): BaseController(logger, mapper)
    {
        [HttpPost("purchase")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PurchaseCart([FromBody] PurchaseCartReq purchaseCartReq)
        {
            var response = new ResultRes<bool>();

            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                {
                    response.Error = "User ID not found";
                    return Unauthorized(response);
                }
                var userId = long.Parse(userIdClaim.Value);

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
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultRes<List<OrderRes>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrders()
        {
            var response = new ResultRes<List<OrderRes>>();

            try
            {
                var userClaimId = User.FindFirst(JwtRegisteredClaimNames.Sub);
                if(userClaimId == null)
                {
                    response.Error = "User ID not found";
                    return Unauthorized(response);
                }
                var userId = long.Parse(userClaimId.Value);

                var orders = await orderService.GetOrders(userId);


                response.Success = true;
                response.Result = Mapper.Map<List<OrderRes>>(orders);
                return Ok(response);

            }
            catch (Exception ex)
            {
                Logger.LogError("Get orders failed: {ex}", ex);
                response.Success = false;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
        /*
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(ResultRes<OrderRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderDetails(long orderId)
        {
            var response = new ResultRes<OrderRes>();

            try
            {
                var orderDetails = await orderService.GetOrderDetails(orderId);
                if (orderDetails == null)
                {
                    return NotFound();
                }

                response.Success = true;
                response.Result = mapper.Map<OrderRes>(orderDetails);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Get order details failed: {ex}", ex);
                response.Success = false;
                response.Error = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }*/

    }
}
