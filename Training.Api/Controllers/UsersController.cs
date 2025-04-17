using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models.Requests.Users;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Users;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Services;
using Training.Common.Helpers;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController(ILogger<UsersController> logger,
        IMapper mapper,
        ICustomerService customerService) : BaseController(logger, mapper)
    {
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromForm] RegisterReq registerRequest)
        {
            var response = new ResultRes<bool>();

            try
            {
                // Check if passwords match
                if (registerRequest.Password != registerRequest.RepeatPassword)
                {
                    response.Error = "Passwords do not match";
                    return BadRequest(response);
                }

                var result = await customerService.RegisterCustomer(Mapper.Map<CustomerDto>(registerRequest));

                if (result)
                {
                    response.Success = true;
                    response.Result = true;
                    return Ok(response);
                }
                else
                {
                    // Handle the case where the email already exists
                    response.Error = "Email already exists";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Error = "Register customer failed";
                Logger.LogError("Register customer failed: {ex}", ex);
                return InternalServerError(response);
            }
        }

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReq changePasswordReq)
        {
            var response = new ResultRes<bool>();

            try
            {
                var userId = this.User.Claims.GetUserId();

                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    response.Error = "User ID not found or token has expired.";
                    return Unauthorized(response);
                }

                if (changePasswordReq.NewPassword != changePasswordReq.RepeatNewPassword)
                {
                    response.Error = "New passwords do not match";
                    return BadRequest(response);
                }

                var changePasswordDto = Mapper.Map<ChangePasswordDto>(changePasswordReq);
                changePasswordDto.Id = userId;

                var result = await customerService.ChangePasswordAsync(changePasswordDto);

                if (!result)
                {
                    response.Error = "Invalid old password";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Result = true;
                return Ok(response);
            }
            catch(Exception ex)
            {
                response.Error = "An error occurred while changing password";
                logger.LogError(ex, "An error occurred while changing password");
                return InternalServerError(response);
            }
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(ResultRes<ProfileRes>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var response = new ResultRes<ProfileRes>();

            try
            {
                var userId = this.User.Claims.GetUserId();

                if (string.IsNullOrEmpty(userId.ToString()))
                {
                    response.Error = "User ID not found or token has expired.";
                    return Unauthorized(response);
                }

                var customerDto = await customerService.GetProfileAsync(userId);

                if (customerDto != null)
                {
                    response.Success = true;
                    response.Result = Mapper.Map<ProfileRes>(customerDto);
                    return Ok(response);
                }
                else
                {
                    response.Error = "Profile not found";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Error = "An error occurred while retrieving the profile";
                logger.LogError(ex, "An error occurred while retrieving the profile.");
                return InternalServerError(response);
            }
        }
    }
}


