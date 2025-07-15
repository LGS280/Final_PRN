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

        public async Task<Employee?> GetEmployeeByIdAsync(string userId)
        {
            return await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeeExistsAsync(employee.EmployeeId))
                    return false;

                throw;
            }
        }

        private async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeId == id);
        }
    }
}
