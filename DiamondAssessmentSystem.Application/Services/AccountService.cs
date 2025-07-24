using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AccountService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var result = new List<AccountDto>();

            foreach (var user in users)
            {
                var roles = await _userRepository.GetUserRolesAsync(user);
                result.Add(new AccountDto
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "Unknown"
                });
            }

            return result;
        }

        public async Task<AccountDto> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            var roles = await _userRepository.GetUserRolesAsync(user);
            return new AccountDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "Unknown"
            };
        }

        public async Task<AccountDto> CreateEmployeeAsync(RegisterEmployeesDto dto, string role)
        {
            var newUser = _mapper.Map<User>(dto);
            newUser.UserType = "Employee";
            newUser.Status = "Active";
            newUser.DateCreated = DateTime.Now;

            var result = await _userRepository.CreateEmployeeWithRoleAsync(newUser, dto.Password, role);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Unable to create staff account: {errorMessages}");
            }

            var roles = await _userRepository.GetUserRolesAsync(newUser);
            return new AccountDto
            {
                UserId = newUser.Id,
                Username = newUser.UserName,
                Email = newUser.Email,
                Role = roles.FirstOrDefault() ?? role
            };
        }

        public async Task<bool> UpdateAccountAsync(string id, AccountDto dto)
        {
            if (id != dto.UserId)
                throw new ArgumentException("User ID mismatch.");

            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return false;

            user.UserName = dto.Username;
            user.Email = dto.Email;
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return false;

            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<bool> UpdateStatusAsync(string userId, string newStatus)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.Status = newStatus;
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}
