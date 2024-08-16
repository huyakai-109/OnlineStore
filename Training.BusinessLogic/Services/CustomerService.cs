using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Dtos.Customers;
using Training.Common.EnumTypes;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services
{
    public interface ICustomerService
    {
        Task<bool> RegisterCustomer(CustomerDto customerDto);
        Task<(string token, CustomerDto customerDto)> LoginAsync(CustomerDto customerDto);

        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        Task<CustomerDto?> GetProfileAsync(long userId);
    }
    public class CustomerService(
        IMapper mapper,
        ITokenService tokenService,
        IUnitOfWork unitOfWork) : ICustomerService
    {
        public async Task<bool> RegisterCustomer(CustomerDto customerDto)
        {
           
            if (customerDto.Password == null)
            {
                throw new ArgumentNullException(nameof(customerDto.Password), "Password cannot be null.");
            }

            var customerRepo = unitOfWork.GetRepository<User>();

            // Check if the email already exists
            if (await customerRepo.Any(c => c.Email == customerDto.Email))
            {
                return false; 
            }

            var user = mapper.Map<User>(customerDto);
            user.Password = CommonHelper.ComputeHash(customerDto.Password);
            user.Role = UserRole.Customer;
            
            await customerRepo.Add(user);
            await unitOfWork.SaveChanges();

            return true; 
        }

        public async Task<(string token, CustomerDto customerDto)> LoginAsync(CustomerDto customerDto)
        {
            if (customerDto.Password == null)
            {
                throw new InvalidOperationException("Password cannot be null.");
            }

            try
            {
                var customerRepo = unitOfWork.GetRepository<User>();

                var user = await customerRepo.Single(c => c.Email == customerDto.Email);

                if (user == null || !CommonHelper.CompareHash(CommonHelper.ComputeHash(customerDto.Password), user.Password))
                {
                    throw new InvalidOperationException("Invalid email or password.");
                }

                var userDto = mapper.Map<CustomerDto>(user);
                var token = tokenService.GenerateToken(userDto);

                return (token, userDto);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }


        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var user = await userRepo.Single(u => u.Id == changePasswordDto.Id);

            if (!string.IsNullOrEmpty(changePasswordDto.OldPassword) && !string.IsNullOrEmpty(changePasswordDto.NewPassword))
            {
                if (user == null || !CommonHelper.CompareHash(CommonHelper.ComputeHash(changePasswordDto.OldPassword), user.Password))
                {
                    return false;
                }

                user.Password = CommonHelper.ComputeHash(changePasswordDto.NewPassword);

                await userRepo.Update(user);
            }
            await unitOfWork.SaveChanges();

            return true;
        }

        public async Task<CustomerDto?> GetProfileAsync(long userId)
        {
           var user = await unitOfWork.GetRepository<User>().FindById(userId);

            if (user == null) return null;

            return mapper.Map<CustomerDto>(user);
        }
    }
}
