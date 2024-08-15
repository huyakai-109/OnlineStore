using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Customers;
using Training.Common.EnumTypes;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;
using ChangePasswordDto = Training.BusinessLogic.Dtos.Admin.ChangePasswordDto;

namespace Training.BusinessLogic.Services.Admin
{
    public interface IUserService
    {
        Task<UserDto?> LoginAsync(UserDto userDto);
		Task SignOutAsync();
		Task CreateDefaultAdminAsync();
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        Task<UserDto?> GetProfile(long id);
        
    }
    public class UserService(IMapper mapper, 
        IUnitOfWork unitOfWork,
        ICookieService cookieService) : IUserService
    {
        public async Task<UserDto?> LoginAsync(UserDto userDto)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var user = await userRepo.Single(u => u.Email == userDto.Email && !u.IsDeleted);
            if (string.IsNullOrEmpty(userDto.Password))
            {
                return null;
            }

            if (user == null || !CommonHelper.CompareHash(CommonHelper.ComputeHash(userDto.Password), user.Password))
            {
                return null;
            }

            if (user.Role == UserRole.Customer)
            {
                return null;
            }

            var rs = mapper.Map<UserDto>(user);
            // Sign in the user
            await cookieService.SignInAsync(rs);

            return rs;

        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var userRepo = unitOfWork.GetRepository<User>();    
            var user = await userRepo.Single(u => u.Id == changePasswordDto.Id);

            if (string.IsNullOrEmpty(changePasswordDto.OldPassword) || string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                return false;   
            }

            if(user == null || !CommonHelper.CompareHash(CommonHelper.ComputeHash(changePasswordDto.OldPassword), user.Password))
                return false;  

            user.Password = CommonHelper.ComputeHash(changePasswordDto.NewPassword);

            await userRepo.Update(user);
            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task CreateDefaultAdminAsync()
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var exists = await userRepo.Any(u => u.Role == UserRole.Admin); 

            if (!exists)
            {
                var defaultAdmin = new User
                {
                    FirstName = "huy",
                    LastName = "truong",
                    CivilianId = "123456789",
                    Email = "admin@gmail.com",
                    Password = "12345".ComputeHash(),
                    PhoneNumber = "123456789",
                    DateOfBirth = DateTime.UtcNow,
                    Role = UserRole.Admin,
                    IsDeleted = false
                };

                await userRepo.Add(defaultAdmin);
                await unitOfWork.SaveChanges();
            }


        }

        public async Task<UserDto?> GetProfile(long userId)
        {
            var user = await unitOfWork.GetRepository<User>().FindById(userId);

            if (user == null) return null;

            return mapper.Map<UserDto>(user);
        }

        public async Task SignOutAsync()
		{
			await cookieService.SignOutAsync();
		}
	}
}
