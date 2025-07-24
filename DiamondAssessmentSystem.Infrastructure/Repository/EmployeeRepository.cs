using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public EmployeeRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.User)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(string userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<User?> GetUserById(int id)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (user == null)
            {
                return null;
            }

            return await _context.Users.FirstOrDefaultAsync(e => e.Id == user.UserId);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            var existingEmployee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == employee.UserId);

            if (existingEmployee == null) return false;

            existingEmployee.Salary = employee.Salary;

            if (employee.User != null)
            {
                existingEmployee.User.FirstName = employee.User.FirstName;
                existingEmployee.User.LastName = employee.User.LastName;
                existingEmployee.User.PhoneNumber = employee.User.PhoneNumber;
                existingEmployee.User.Gender = employee.User.Gender;
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeeExistsAsync(existingEmployee.EmployeeId))
                    return false;

                throw;
            }
        }

        private async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
        }

        public async Task<string?> GetEmployeeEmail(string userId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return employee?.User?.Email;
        }
    }
}
