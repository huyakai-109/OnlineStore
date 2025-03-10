using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Training.BusinessLogic.Dtos.Admin;
using Training.Common.Constants;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services.Admin
{
    public interface IUserService
    {
        Task<UserDto?> LoginAsync(UserDto userDto);
        Task SignOutAsync();
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        Task<UserDto?> GetProfile(long id);

    }
    public class UserService(IMapper mapper,
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ICookieService cookieService) : IUserService
    {
        private readonly int _maxFailedAccessAttempts = configuration.GetSection(ConfigKeys.Security.Lockout.MaxFailedAccessAttempts).Get<int>();
        private readonly int _defaultLockoutMinutes = configuration.GetSection(ConfigKeys.Security.Lockout.DefaultLockoutMinutes).Get<int>();

        public async Task<UserDto?> LoginAsync(UserDto userDto)
        {
            var user = await userManager.FindByNameAsync(userDto.Email!);
            if (user == null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            var result = await signInManager.PasswordSignInAsync(userDto.Email!, userDto.Password!, true, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    return null;
                }

                if (user.AccessFailedCount + 1 == _maxFailedAccessAttempts)
                {
                    return null;
                }

                if (user.AccessFailedCount == _maxFailedAccessAttempts)
                {
                    var lockoutTime = DateTimeHelper.GetDtOffset().AddMinutes(_defaultLockoutMinutes);
                    await userManager.SetLockoutEndDateAsync(user, lockoutTime);

                    return null;
                }

                return null;
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

            var info = new UserDto();
            info.IsAdmin = user.IsAdmin;

            var employee = await unitOfWork.GetRepository<Employee>().Single(i => i.UserId == user.Id);
            if (employee != null)
            {
                info.FirstName = employee.FirstName;
                info.LastName = employee.LastName;
                userClaims.Add(new Claim(UserConstants.Claim.Name, employee.FirstName));
            }

            // Sign in the user
            await cookieService.SignInAsync(userClaims);

            return info;

        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await userManager.FindByIdAsync(changePasswordDto.Id.ToString());
            if (user == null)
            {
                return false;
            }

            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword!, changePasswordDto.NewPassword!);

            return result.Succeeded;
        }

        public async Task<UserDto?> GetProfile(long userId)
        {
            var user = await unitOfWork.GetRepository<Employee>().Single(i => i.UserId == userId);

            if (user == null) return null;

            return mapper.Map<UserDto>(user);
        }

        public async Task SignOutAsync()
        {
            await cookieService.SignOutAsync();
        }
    }
}
