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


namespace Training.BusinessLogic.Services
{
    public interface ITokenService
    {
        string GenerateToken(CustomerDto user);

    }
    public class TokenService(IConfiguration _configuration) : ITokenService
    {
        public string GenerateToken(CustomerDto user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[ConfigKeys.Security.Jwt.Secret]));
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
        
    }
}
