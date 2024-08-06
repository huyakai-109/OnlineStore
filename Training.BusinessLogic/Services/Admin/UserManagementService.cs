using AutoMapper;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.BusinessLogic.Common;
using Training.BusinessLogic.Dtos.Admin;
using Training.BusinessLogic.Dtos.Base;
using Training.BusinessLogic.Dtos.Customers;
using Training.Common.EnumTypes;
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
            var user = mapper.Map<User>(userDto);
            user.Password = CommonHelper.ComputeHash(userDto.Password);
            user.CreatedAt = DateTime.UtcNow;

            await unitOfWork.GetRepository<User>().Add(user);
            await unitOfWork.SaveChanges();
        }

        public async Task DeleteUser(long Id)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var user = await userRepo.Single(u => u.Id == Id);

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
            var user = await unitOfWork.GetRepository<User>().FindById(id);
            if (user == null) return null;

            return mapper.Map<UserDto>(user);
        }

        public async Task<(List<UserDto> Users, Pagination Pagination)> GetUsers(CommonSearchDto search)
        {
            var query = await unitOfWork.GetRepository<User>().QueryCondition(u => !u.IsDeleted);

            if (!string.IsNullOrEmpty(search.SearchQuery))
            {
                UserRole? roleEnum = EnumHelper<UserRole>.ToEnum(search.SearchQuery);
                    
                var searchLower = search.SearchQuery.ToLower();
                query = query.Where(u => u.FirstName.ToLower() == searchLower
                                      || u.LastName.ToLower() == searchLower
                                      || u.Email.ToLower() == searchLower
                                      || (roleEnum.HasValue && u.Role == roleEnum.Value));

            }

            var totalCount = await query.CountAsync();
            var users = await query.Skip((search.Skip - 1) * search.Take).Take(search.Take).ToListAsync();

            var pagination = new Pagination(totalCount, users.Count, search.Skip, search.Take);

            return (mapper.Map<List<UserDto>>(users), pagination);
        }

        public async Task<bool> UpdateUser(UserDto userDto)
        {
            var userRepo = unitOfWork.GetRepository<User>();
            var user = await userRepo.Single(u => u.Id == userDto.Id);
            if (user == null) 
            { 
                return false; 
            }

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.Role = userDto.Role;
            user.CivilianId = userDto.CivilianId;
            user.DateOfBirth = userDto.DateOfBirth;
            user.PhoneNumber = userDto.PhoneNumber;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = CommonHelper.ComputeHash(userDto.Password);
            }

            await userRepo.Update(user);
            await unitOfWork.SaveChanges();

            return true;


        }
    }
}
