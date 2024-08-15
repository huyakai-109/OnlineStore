 using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Dtos.Admin;
using Training.Common.Constants;
using Training.Common.EnumTypes;
using Microsoft.Extensions.Logging;

namespace Training.BusinessLogic.Services.Admin
{
    public interface ICookieService
    {
        Task SignInAsync(UserDto userDto);
        Task SignOutAsync();
    }
    public class CookieService(IHttpContextAccessor _httpContextAccessor, ILogger<CookieService> logger) : ICookieService
    {
        private HttpContext GetHttpContext()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }
            return context;
        }
        public async Task SignInAsync(UserDto userDto)
        {
            try
            {
                var context = GetHttpContext();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{userDto.FirstName} {userDto.LastName}"),
                    new Claim(ClaimTypes.Email, userDto.Email ?? string.Empty),
                    new Claim(RolePolicies.ClaimType, userDto.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, " ");
                throw;
            }
        }

        public async Task SignOutAsync()
        {
            var context = GetHttpContext();
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
