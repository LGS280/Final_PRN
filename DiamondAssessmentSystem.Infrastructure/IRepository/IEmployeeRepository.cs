﻿using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(string userId);
        Task<User?> GetUserById(int id);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<string?> GetEmployeeEmail(string userId);
    }
}
