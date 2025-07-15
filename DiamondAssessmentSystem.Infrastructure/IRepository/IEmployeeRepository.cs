using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeByIdAsync(string userId);
        Task<bool> UpdateEmployeeAsync(Employee employee);
    }
}
