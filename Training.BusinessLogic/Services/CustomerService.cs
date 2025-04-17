using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Training.BusinessLogic.Dtos.Customers;
using Training.BusinessLogic.Dtos.Storage;
using Training.Common.Constants;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services
{
    public interface ICustomerService
    {
        Task<bool> RegisterCustomer(CustomerDto customerDto);

        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);

        Task<CustomerDto?> GetProfileAsync(long userId);
    }
    public class CustomerService(
        IMapper mapper,
        UserManager<User> userManager,
        IStorageService storageService,
        IUnitOfWork unitOfWork) : ICustomerService
    {
        public async Task<bool> RegisterCustomer(CustomerDto customerDto)
        {
            var customerRepo = unitOfWork.GetRepository<Customer>();
            var userRepo = unitOfWork.GetRepository<User>();
            var userRoleRepo = unitOfWork.GetRepository<UserRole>();

            if (await customerRepo.Any(c => c.Email == customerDto.Email))
            {
                return false;
            }

            var strategy = unitOfWork.DbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await unitOfWork.DbContext.Database.BeginTransactionAsync();
                try
                {
                    var customer = new Customer()
                    {
                        Email = customerDto.Email,
                        FirstName = customerDto.FirstName,
                        LastName = customerDto.LastName,
                        PhoneNumber = customerDto.PhoneNumber,
                        DateOfBirth = customerDto.DateOfBirth,
                        User = new User()
                        {
                            Email = customerDto.Email,
                            NormalizedEmail = customerDto.Email,
                            UserName = customerDto.Email,
                            NormalizedUserName = customerDto.Email,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            UserRoles = new List<UserRole>()
                            {
                                new UserRole()
                                {
                                    RoleId = RolePolicies.Customer.Id,
                                }
                            },
                            PasswordHash = customerDto.Password!.HashPassword(),
                            IsActive = true,
                            LockoutEnabled = true,
                        }
                    };

                    if(customerDto.Avatar !=null && customerDto.Avatar.Length > 0)
                    {
                        var uploadFileReqDto = new UploadFileReqDto
                        {
                            File = customerDto.Avatar,
                            Metadata = new Dictionary<string, string>
                            {
                                { "Source", "Training" },
                                { "TypeFile", "Customer Avatar" }
                            }
                        };
                        var (success, fileResult) = await storageService.Upload(uploadFileReqDto);
                        if (success)
                        {
                            customer.Avatar = fileResult;
                        }
                    }

                    await customerRepo.Add(customer);
                    await unitOfWork.SaveChanges();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    if(transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                }
                finally
                {
                    unitOfWork.Dispose();
                }
            });

            return true;
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

        public async Task<CustomerDto?> GetProfileAsync(long userId)
        {
            var customer = await (from us in await unitOfWork.GetRepository<User>().QueryAll()
                           join cus in await unitOfWork.GetRepository<Customer>().QueryAll()
                           on us.Id equals cus.UserId
                           where us.Id == userId
                           select cus).FirstOrDefaultAsync();

            if (customer == null) return null;

            return mapper.Map<CustomerDto>(customer);
        }
    }
}
