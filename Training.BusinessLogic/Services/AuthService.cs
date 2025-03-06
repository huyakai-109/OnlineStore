using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Training.BusinessLogic.Dtos.Auth;
using Training.Common.Constants;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;
using Tricor.BillingProcess.Common.Messages;

namespace Training.BusinessLogic.Services
{
    public interface IAuthService
    {
        Task<(string, LoginResultDto)> Login(LoginReqDto request);

        Task<bool> Logout(LogoutReqDto request);
    }

    public class AuthService(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<AuthService> logger) : IAuthService
    {
        private readonly string _secret = configuration.GetSection(ConfigKeys.Security.Jwt.Secret).Get<string>()!;
        private readonly string _refreshSecret = configuration.GetSection(ConfigKeys.Security.Jwt.RefreshSecret).Get<string>()!;
        private readonly int _expirationMinutes = configuration.GetSection(ConfigKeys.Security.Jwt.ExpirationMinutes).Get<int>();
        private readonly int _refreshExpirationDays = configuration.GetSection(ConfigKeys.Security.Jwt.RefreshExpirationDays).Get<int>();
        private readonly int _maxFailedAccessAttempts = configuration.GetSection(ConfigKeys.Security.Lockout.MaxFailedAccessAttempts).Get<int>();
        private readonly int _defaultLockoutMinutes = configuration.GetSection(ConfigKeys.Security.Lockout.DefaultLockoutMinutes).Get<int>();

        public async Task<(string, LoginResultDto)> Login(LoginReqDto request)
        {
            var loginResult = new LoginResultDto();

            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return (AuthControllerMS.Login.InvalidCredential, loginResult);
            }

            if (!user.IsActive)
            {
                return (AuthControllerMS.Login.InActive, loginResult);
            }

            var result = await signInManager.PasswordSignInAsync(request.Username, request.Password, true, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    loginResult.LockoutEnd = user.LockoutEnd.HasValue ? user.LockoutEnd.Value.AddHours(7).DateTime : null;
                    loginResult.AccessFailedCount = _maxFailedAccessAttempts;
                    return (AuthControllerMS.Login.LockedOut, loginResult);
                }

                loginResult.AccessFailedCount = user.AccessFailedCount;

                if (user.AccessFailedCount + 1 == _maxFailedAccessAttempts)
                {
                    return (AuthControllerMS.Login.WillBeLockedOut, loginResult);
                }

                if (user.AccessFailedCount == _maxFailedAccessAttempts)
                {
                    var lockoutTime = DateTimeHelper.GetDtOffset().AddMinutes(_defaultLockoutMinutes);
                    await userManager.SetLockoutEndDateAsync(user, lockoutTime);
                    loginResult.LockoutEnd = user.LockoutEnd.HasValue ? user.LockoutEnd.Value.AddHours(7).DateTime : null;
                    loginResult.AccessFailedCount = _maxFailedAccessAttempts;
                    return (AuthControllerMS.Login.LockedOut, loginResult);
                }

                return (AuthControllerMS.Login.InvalidCredential, loginResult);
            }

            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            user.LastLogin = DateTime.Now;


            var principal = await signInManager.CreateUserPrincipalAsync(user);
            var userClaims = principal.Claims.ToList();

            var roleQuery = await unitOfWork.GetRepository<Role>().QueryAll();
            var userRoleQuery = await unitOfWork.GetRepository<UserRole>().QueryAll();
            var roleClaimQuery = await unitOfWork.GetRepository<RoleClaim>().QueryAll();

            var authorizationClaims = (from ur in userRoleQuery
                                       join r in roleQuery on ur.RoleId equals r.Id
                                       join rc in roleClaimQuery on r.Id equals rc.RoleId
                                       where ur.UserId == user.Id
                                       select rc.ClaimValue).Distinct().ToArray();

            userClaims.AddRange(authorizationClaims.Select(ac => new Claim(RolePolicies.ClaimType, ac)).ToList());

            var employee = await unitOfWork.GetRepository<Employee>().Single(i => i.UserId == user.Id);
            if (employee != null)
            {
                userClaims.Add(new Claim(UserConstants.Claim.Name, employee.FirstName));
            }

            var token = GenerateToken(userClaims, _secret, DateTimeHelper.GetDt().AddMinutes(_expirationMinutes));
            var refreshExpiration = DateTimeHelper.GetDt().AddDays(_refreshExpirationDays);
            var refreshToken = GenerateToken( userClaims,_refreshSecret,refreshExpiration);

            await userManager.UpdateAsync(user);

            var userToken = await unitOfWork.GetRepository<UserToken>().Single(i => i.DeviceUuid == request.DeviceUuid && i.UserId == user.Id);
            if (userToken == null)
            {
                userToken = new UserToken()
                {
                    UserId = user.Id,
                    DeviceUuid = request.DeviceUuid,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = refreshExpiration,
                    Token = token,
                    LoginProvider = GlobalConstants.JWTLoginToken,
                    Name = $"{user.UserName}_{DateTime.Now.ToFileTime()}",
                };
                await unitOfWork.GetRepository<UserToken>().Add(userToken);
            }
            else
            {
                userToken.Token = token;
                userToken.RefreshToken = refreshToken;
                userToken.RefreshTokenExpiryTime = refreshExpiration;
                await unitOfWork.GetRepository<UserToken>().Update(userToken);
            }
            await unitOfWork.SaveChanges();

            loginResult.Token = token;
            loginResult.RefreshToken = refreshToken;
            return (string.Empty, loginResult);
        }

        public async Task<bool> Logout(LogoutReqDto request)
        {
            try
            {
                var user = await VerifyToken(request.Token, _secret);
                if (user == null)
                {
                    return false;
                }

                user = await VerifyUserToken(request.DeviceUuid, request.Token, request.RefreshToken);
                if (user == null)
                {
                    return false;
                }

                var userToken = (await unitOfWork.GetRepository<UserToken>().QueryCondition(i => i.UserId == user.Id
                    && i.Token == request.Token
                    && i.RefreshToken == request.RefreshToken)).First();

                await unitOfWork.GetRepository<UserToken>().Delete(new object[] { userToken.UserId, userToken.LoginProvider, userToken.Name });
                await unitOfWork.SaveChanges();

                return true;
            }
            catch (SecurityTokenValidationException ex)
            {
                logger.LogError("Token is not valid {ex}", ex);
                return false;
            }
        }

        private string GenerateToken(IEnumerable<Claim> claims, string secret, DateTime expireDateTime)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireDateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)), SecurityAlgorithms.HmacSha256Signature),
                Audience = configuration.GetSection(ConfigKeys.Security.Jwt.Audience).Get<string>(),
                Issuer = configuration.GetSection(ConfigKeys.Security.Jwt.Issuer).Get<string>()
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<User?> VerifyToken(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = ReadJwtToken(token);

            var userId = jwtToken.Claims.GetUserId();

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            var securityKey = secret.ToByteArray();

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(securityKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return user;
        }

        private async Task<User?> VerifyUserToken(Guid deviceUuid, string token, string refreshToken)
        {
            var tokenContent = ReadJwtToken(token);
            var refreshTokenContent = ReadJwtToken(refreshToken);

            var userId = tokenContent.Claims.GetUserId();
            var refreshTokenUserId = refreshTokenContent.Claims.GetUserId();

            if (userId != refreshTokenUserId)
            {
                logger.LogWarning("User of both token aren't matched: {userId} - {refreshTokenUserId}", userId, refreshTokenUserId);
                return null;
            }

            var user = await VerifyToken(refreshToken, _refreshSecret);
            if (user == null)
            {
                return null;
            }

            var userToken = await unitOfWork.GetRepository<UserToken>().Single(i => i.UserId == user.Id && i.DeviceUuid == deviceUuid);
            if (userToken == null
                || userToken.RefreshToken != refreshToken
                || userToken.RefreshTokenExpiryTime < DateTimeHelper.GetDtOffset())
            {
                return null;
            }

            return user;
        }

        private static JwtSecurityToken ReadJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken;
        }
    }
}
