using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class ServicePriceRepository : IServicePriceRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public ServicePriceRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServicePrice>> GetAllAsync()
        {
            return await _context.ServicePrices.ToListAsync();
        }

        public async Task<IEnumerable<ServicePrice>> GetByStatusAsync(string status)
        {
            return await _context.ServicePrices
                                 .Where(sp => sp.Status == status)
                                 .ToListAsync();
        }

        public async Task<ServicePrice?> GetByIdAsync(int id)
        {
            return await _context.ServicePrices.FindAsync(id);
        }

        public async Task<ServicePrice> AddAsync(ServicePrice servicePrice)
        {
            _context.ServicePrices.Add(servicePrice);
            await _context.SaveChangesAsync();
            return servicePrice;
        }

        public async Task<bool> UpdateAsync(ServicePrice servicePrice)
        {
            _context.Entry(servicePrice).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(servicePrice.ServiceId))
                    return false;

                throw;
            }
        }

        private async Task<bool> ExistsAsync(int id)
        {
            return await _context.ServicePrices.AnyAsync(e => e.ServiceId == id);
        }

        public async Task<int> GetEmployeeId(string userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == userId);

            if (employee == null)
            {
                return -1;
            }

            return employee.EmployeeId;
        }
    }
}
