using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Training.Api.Models.Requests.Carts;
using Training.Api.Models.Responses.Base;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Services;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class CartsController(ILogger<CartsController> logger,
         IMapper mapper,
         ICartService cartService) : BaseController(logger, mapper)
    {

        [Authorize]
        [HttpPost("edit-quantity")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditQuantity(EditQuantityReq editQuantityReq)
        {
            var response = new ResultRes<bool>();

            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (userIdClaim == null)
                {
                    response.Error = "User ID not found";
                    return BadRequest(response);
                }
                var userId = long.Parse(userIdClaim.Value);

                var editCartItemDto = Mapper.Map<EditCartQuantityDto>(editQuantityReq);
                editCartItemDto.UserId = userId;

                var result = await cartService.EditQuantity(editCartItemDto);

                if (!result)
                {
                    response.Error = "Failed to edit cart item quantity";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Result = result;

                return Ok(response);    
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError("Edit product's quantity failed: {ex}", ex);
                response.Success = false;
                response.Error = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Edit product's quantity failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
        [Authorize]
        [HttpPost("remove-product")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProduct(RemoveProductFCartReq removeProductFCartReq)
        {
            var response = new ResultRes<bool>();
            try
            {
                var UserIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);
                if(UserIdClaim == null)
                {
                    response.Error = "User ID not found";
                    return BadRequest(response);
                }
                var userId = long.Parse(UserIdClaim.Value);

                var removeProductFCartDto = Mapper.Map<RemoveProductFCartDto>(removeProductFCartReq);
                removeProductFCartDto.UserId = userId;

                var result = await cartService.RemoveProduct(removeProductFCartDto);

                if (!result)
                {
                    response.Error = "Failed to remove product from cart";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Result = result;
                return Ok(response);    
            }
            catch (Exception ex)
            {
                Logger.LogError("Remove product failed: {ex}", ex);
                response.Success = false;
                return InternalServerError(response);
            }
        }
    }
}
