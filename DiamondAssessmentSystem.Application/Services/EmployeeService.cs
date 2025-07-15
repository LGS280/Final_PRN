using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _accountRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IUserRepository accountRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto?> GetEmployees(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var employee = await _employeeRepository.GetEmployeeByIdAsync(userId);

            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> UpdateEmployee(string userId, EmployeeDto employeeDto)
        {
            if (string.IsNullOrWhiteSpace(userId) || employeeDto == null)
                return false;

            var existingEmployee = await _employeeRepository.GetEmployeeByIdAsync(userId);

            if (existingEmployee == null)
                return false;

            _mapper.Map(employeeDto, existingEmployee);

            return await _employeeRepository.UpdateEmployeeAsync(existingEmployee);
        }
    }
}
