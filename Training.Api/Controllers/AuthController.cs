using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.Api.Models.Requests.Users;
using Training.Api.Models.Responses.Base;
using Training.Api.Models.Responses.Users;
using Training.BusinessLogic.Dtos.Auth;
using Training.BusinessLogic.Services;
using Tricor.BillingProcess.Common.Messages;

namespace Training.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController (
        ILogger<AuthController> logger,
        IMapper mapper,
        IAuthService authService) : BaseController(logger, mapper)
    {
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(ResultRes<LoginRes>), StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginReq request)
        {
            var response = new ResultRes<LoginRes>();

            try
            {
                if (string.IsNullOrWhiteSpace(request.Username)
                    || string.IsNullOrEmpty(request.Password))
                {
                    response.Error = AuthControllerMS.Login.InvalidCredential;
                    return BadRequest(response);
                }

                var (errorCode, loginResult) = await authService.Login(Mapper.Map<LoginReqDto>(request));
                response.Result = Mapper.Map<LoginRes>(loginResult);
                if (!string.IsNullOrWhiteSpace(errorCode))
                {
                    response.Error = errorCode;
                    return BadRequest(response);
                }

                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Error = AuthControllerMS.Login.Exception;
                Logger.LogError("Login failed: {ex}", ex);
                return InternalServerError(response);
            }
        }

        [HttpPut]
        [Route("logout")]
        [ProducesResponseType(typeof(ResultRes<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout(LogoutReq logoutReq)
        {
            var response = new ResultRes<bool>();

            try
            {
                if (string.IsNullOrEmpty(logoutReq.Token) || string.IsNullOrEmpty(logoutReq.RefreshToken))
                {
                    response.Error = AuthControllerMS.Logout.RequiredToken;
                    return BadRequest(response);
                }

                response.Success = true;
                var loginResult = await authService.Logout(Mapper.Map<LogoutReqDto>(logoutReq));
            }
            catch (Exception ex)
            {
                Logger.LogError("Logout failed: {ex}", ex);
            }

            return Ok(response);
        }
    }
}
