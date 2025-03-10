using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using Training.BusinessLogic.Common;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.Common.Helpers;
using Training.DataAccess.Entities;
using Training.Repository.UoW;

namespace Training.BusinessLogic.Services.Admin
{
    public interface IUserManagementService
    {
        Task<(List<UserDto> Users, Pagination Pagination)> GetUsers(CommonSearchDto search);

        Task CreateUser(UserDto userDto);

        Task<UserDto?> GetUserById(long id);

        Task<bool> UpdateUser(UserDto userDto);

        Task DeleteUser(long Id);

        byte[] ExportUsersToCsv(List<UserDto> users);

        Task<List<UserDto>> GetAllUsers();
    }
    public class UserManagementService(IMapper mapper,
        IUnitOfWork unitOfWork) : IUserManagementService
    {
        public async Task CreateUser(UserDto userDto)
        {
            var employeeRepo = unitOfWork.GetRepository<Employee>();

            /*var isExistEmail = await unitOfWork.GetRepository<User>().Any(i => i.Email == userDto.UserName);

             add Validate role in the future 
           
                var existedRole = (await unitOfWork.GetRepository<Role>()
                .QueryCondition(i => requestDto.RoleIds.Contains(i.Id)))
                .Select(i => i.Id)
                .ToArray();
            if (requestDto.RoleIds.Except(existedRole).Any())
            {
                
            }
             */

            var userRoles = new List<DataAccess.Entities.UserRole>();
            foreach (var roleId in userDto.RoleIds!)
            {
                userRoles.Add(new DataAccess.Entities.UserRole()
                {
                    RoleId = roleId
                });
            }

            var employee = new Employee()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
                DateOfBirth = userDto.DateOfBirth,
                User = new User()
                {
                    Email = userDto.Email,
                    NormalizedEmail = userDto.Email,
                    UserName = userDto.Email,
                    NormalizedUserName = userDto.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserRoles = userRoles,
                    PasswordHash = userDto.Password!.HashPassword(),
                    IsActive = true,
                    LockoutEnabled = true,
                }
            };

            await unitOfWork.GetRepository<Employee>().Add(employee);
            await unitOfWork.SaveChanges();
        }

        public async Task DeleteUser(long Id)
        {
            var userRepo = unitOfWork.GetRepository<Employee>();
            var user = await userRepo.Single(u => u.Id == Id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Product with Id {Id} not found.");
            }
            user.IsDeleted = true;

            await userRepo.Update(user);
            await unitOfWork.SaveChanges();
        }

        public byte[] ExportUsersToCsv(List<UserDto> users)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(streamWriter, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
            {
                csvWriter.WriteRecords(users);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await unitOfWork.GetRepository<User>().QueryAll();

            return mapper.Map<List<UserDto>>(await users.ToListAsync());
        }

        public async Task<UserDto?> GetUserById(long id)
        {
            var employee = await (from emp in await unitOfWork.GetRepository<Employee>().QueryAll()
                                  join us in await unitOfWork.GetRepository<User>().QueryAll()
                                  on emp.UserId equals us.Id
                                  join ur in await unitOfWork.GetRepository<UserRole>().QueryAll()
                                  on us.Id equals ur.UserId
                                  join r in await unitOfWork.GetRepository<Role>().QueryAll()
                                  on ur.RoleId equals r.Id
                                  where emp.Id == id
                                  select new UserDto()
                                  {
                                      Id = emp.Id,
                                      FirstName = emp.FirstName,
                                      LastName = emp.LastName,
                                      Email = us.Email,
                                      PhoneNumber = emp.PhoneNumber,
                                      RoleName = r.DisplayName,
                                  }).FirstOrDefaultAsync();
            if(employee == null)
            {
                return null;
            }

            return employee;
        }

        public async Task<(List<UserDto> Users, Pagination Pagination)> GetUsers(CommonSearchDto search)
        {
            var query = from emp in await unitOfWork.GetRepository<Employee>().QueryAll()
                        join us in await unitOfWork.GetRepository<User>().QueryAll()
                        on emp.UserId equals us.Id
                        join ur in await unitOfWork.GetRepository<UserRole>().QueryAll()
                        on us.Id equals ur.UserId
                        join r in await unitOfWork.GetRepository<Role>().QueryAll()
                        on ur.RoleId equals r.Id
                        select new UserDto()
                        {
                            Id = emp.Id,
                            FirstName = emp.FirstName,
                            LastName = emp.LastName,
                            Email = us.Email,
                            PhoneNumber = emp.PhoneNumber,
                            RoleName = r.DisplayName,
                        };

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {

                var searchLower = search.SearchQuery.ToLower();
                query = query.Where(u => u.FirstName!.ToLower() == searchLower
                                      || u.LastName!.ToLower() == searchLower
                                      || u.Email!.ToLower() == searchLower);

            }

            var totalCount = await query.CountAsync();
            var users = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, users.Count, search.Skip, search.Take);

            return (mapper.Map<List<UserDto>>(users), pagination);
        }

        public async Task<bool> UpdateUser(UserDto userDto)
        {
            var employeeRepo = unitOfWork.GetRepository<Employee>();
            var userRepo = unitOfWork.GetRepository<User>();
            var userRoleRepo = unitOfWork.GetRepository<UserRole>();

            var employee = await employeeRepo.Single(u => u.Id == userDto.Id);
            if (employee == null)
            {
                return false;
            }

            var user = await userRepo.Single(u => u.Id == employee.UserId);
            if (user == null)
            {
                return false;
            }

            var roles = (await userRoleRepo.QueryCondition(i => i.UserId == user.Id)).ToArray();
            if (roles.Length > 0)
            {
                await userRoleRepo.Delete(roles);
            }

            if (userDto.RoleIds!.Length > 0)
            {
                var newRole = userDto.RoleIds.Select(i => new UserRole()
                {
                    UserId = user.Id,
                    RoleId = i
                });

                await userRoleRepo.Add(newRole);
            }

            employee.FirstName = userDto.FirstName;
            employee.LastName = userDto.LastName;
            employee.DateOfBirth = userDto.DateOfBirth;
            employee.PhoneNumber = userDto.PhoneNumber;
            user.Email = userDto.Email;

            await userRepo.Update(user);
            await employeeRepo.Update(employee);
            await unitOfWork.SaveChanges();

            return true;
        }
    }
}
