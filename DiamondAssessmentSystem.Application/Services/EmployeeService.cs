using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository userRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetEmployees(string userId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(userId);
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<AccountDto?> GetUserById(int id)
        {
            var user = await _employeeRepository.GetUserById(id);
            return user == null ? null : _mapper.Map<AccountDto>(user);
        }

        public async Task<bool> UpdateEmployee(string userId, EmployeeUpdateDto dto)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(userId);
            if (employee == null) return false;

            employee.Salary = dto.Salary ?? 0;

            if (employee.User != null)
            {
                employee.User.FirstName = dto.FirstName;
                employee.User.LastName = dto.LastName;
                employee.User.PhoneNumber = dto.Phone;
                employee.User.Gender = dto.Gender;
            }

            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(string userId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(userId);
            if (employee == null) return false;

            await _userRepository.DeleteUserAsync(userId);
            return true;
        }

        public async Task<string?> GetEmployeeEmail(string userId)
        {
            return await _employeeRepository.GetEmployeeEmail(userId);
        }
    }
}
