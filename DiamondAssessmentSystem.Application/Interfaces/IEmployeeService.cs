using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployees(string userId);
        Task<AccountDto?> GetUserById(int id);
        Task<bool> UpdateEmployee(string userId, EmployeeUpdateDto dto);
        Task<bool> DeleteEmployeeAsync(string userId);
        Task<string?> GetEmployeeEmail(string userId);
    }
}
