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

namespace Training.BusinessLogic.Services.Admin
{
    public interface ICookieService
    {
        Task SignInAsync(UserDto userDto);
        Task SignOutAsync();
    }
    public class CookieService(IHttpContextAccessor _httpContextAccessor) : ICookieService
    {
        public async Task SignInAsync(UserDto userDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.FirstName +" " + userDto.LastName),
                new Claim(ClaimTypes.Email, userDto.Email),
                new Claim(ClaimTypes.Role, userDto.Role.ToString()),
                new Claim(RolePolicies.ClaimType, userDto.Role == UserRole.Admin ? "Admin" : "Clerk")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                // not required  IsPersistent = isPersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                                new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
