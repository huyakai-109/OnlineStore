using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterReq registerRequest)
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
            catch (ArgumentNullException ex)
            {
                Logger.LogWarning("Registration failed due to null value: {ex}", ex);
                response.Error = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Registration unsuccessful: {ex}", ex);
                response.Error = "An error occurred while processing your request";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResultRes<LoginRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<LoginRes>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultRes<LoginRes>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginReq loginReq)
        {
            var response = new ResultRes<LoginRes>();

            try
            {
                var (token, user) = await customerService.LoginAsync(Mapper.Map<CustomerDto>(loginReq));

                response.Success = true;
                response.Result = new LoginRes { Token = token, User = user };
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogWarning("Invalid login attempt: {ex}", ex);
                response.Success = false;
                response.Error = ex.Message;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                Logger.LogError("Login unsuccessful: {ex}", ex);
                response.Success = false;
                response.Error = "An error occurred while processing your request";
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReq changePasswordReq)
        {
            var response = new ResultRes<bool>();
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var userIdLong))
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
            changePasswordDto.Id = userIdLong;

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

        [HttpGet("profile")]
        [ProducesResponseType(typeof(ResultRes<ProfileRes>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultRes<ProfileRes>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProfile()
        {
            var response = new ResultRes<ProfileRes>();

            try
            {
              
                var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null)
                {
                    response.Error = "User ID not found or token has expired.";
                    return Unauthorized(response);
                }

                var customerDto = await customerService.GetProfileAsync(long.Parse(userIdClaim));

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
                logger.LogError(ex, "An error occurred while retrieving the profile.");
                response.Error = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }

        }
    }

}


