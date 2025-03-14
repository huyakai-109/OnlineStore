using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Cart;
using Training.Api.Models.Responses.Payments;
using Training.BusinessLogic.Services;
using Training.Common.Helpers;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class PaymentController(
        ILogger<PaymentController> logger,
        IMapper mapper,
        IPaymentService paymentService) : BaseController(logger, mapper)
    {
        [HttpPost]
        [Route("init-payment-intent")]
        [ProducesResponseType(typeof(ResultRes<CartRes>), StatusCodes.Status200OK)]
        public async Task<IActionResult> InitPaymentIntent()
        {
            var response = new ResultRes<CartRes>();

            try
            {
                var userId = this.User.Claims.GetUserId();
                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    response.Error = "User ID not found";
                    return Unauthorized(response);
                }

                var result = await paymentService.CreateOrUpdatePaymentIntent(userId);
                if (result == null)
                {
                    response.Error = "Failed to init payment";
                    return BadRequest(response);
                }

                response.Result = Mapper.Map<CartRes>(result);
                response.Success = true;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Init payment failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }

        [HttpGet]
        [Route("delivery-methods")]
        [ProducesResponseType(typeof(ResultRes<DeliveryMethodRes[]>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeliveryMethods()
        {
            var response = new ResultRes<DeliveryMethodRes[]>();

            try
            {
                var result = await paymentService.GetAllDeliveryMethod();

                response.Result = Mapper.Map<DeliveryMethodRes[]>(result);
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Get all delivery methods failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
    }
}
