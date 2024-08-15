using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Dtos.Customers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Training.Common.Constants;
using Training.BusinessLogic.Dtos.Admin;
using Microsoft.Extensions.Logging;


namespace Training.BusinessLogic.Services
{
    public interface ITokenService
    {
        string GenerateToken(CustomerDto user);

    }
    public class TokenService(IConfiguration _configuration, ILogger<TokenService> logger) : ITokenService
    {
        public string GenerateToken(CustomerDto user)
        {
            try 
            {
                var claims = new[]
                {
                     new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                     new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

                };

                var secret = _configuration[ConfigKeys.Security.Jwt.Secret];
                if (string.IsNullOrEmpty(secret))
                {
                    throw new InvalidOperationException("JWT Secret is not configured.");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration[ConfigKeys.Security.Jwt.Issuer],
                    audience: _configuration[ConfigKeys.Security.Jwt.Audience],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration[ConfigKeys.Security.Jwt.ExpirationMinutes])),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, " ");
                throw;
            }
           
        }
        
    }
}
