using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Enums;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;
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

        public async Task<EmployeeEnum> UpdateEmployee(string userId, EmployeeUpdateDto employeeDto)
        {
            if (string.IsNullOrWhiteSpace(userId) || employeeDto == null)
                return EmployeeEnum.NotFound;

            var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(userId);
            if (existingEmployee == null)
                return EmployeeEnum.NotFound;

            if (employeeDto.Phone != null && !IsPhoneNumberValid(employeeDto.Phone, "VN"))
                return EmployeeEnum.InvalidPhoneNumber;

            // Map thủ công
            existingEmployee.Salary = employeeDto.Salary;
            if (existingEmployee.User != null)
            {
                existingEmployee.User.FirstName = employeeDto.FirstName;
                existingEmployee.User.LastName = employeeDto.LastName;
                existingEmployee.User.PhoneNumber = employeeDto.Phone;
                existingEmployee.User.Gender = employeeDto.Gender;
            }

            var updateSuccess = await _employeeRepository.UpdateEmployeeAsync(existingEmployee);
            return updateSuccess ? EmployeeEnum.Success : EmployeeEnum.UpdateFailed;
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

        private bool IsPhoneNumberValid(string phoneNumber, string regionCode)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            try
            {
                var phoneNumberUtil = PhoneNumberUtil.GetInstance();
                var parsedNumber = phoneNumberUtil.Parse(phoneNumber, regionCode);
                return phoneNumberUtil.IsValidNumber(parsedNumber);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
