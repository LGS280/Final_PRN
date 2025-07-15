using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public CustomerRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCustomerIdAsync(string userId)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return customer?.CustomerId ?? -1;
        }

        public async Task<Customer?> GetCustomerByIdAsync(string userId)
        {
            return await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            _context.Entry(customer.User).State = EntityState.Modified;
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExistsAsync(customer.CustomerId))
                {
                    return false;
                }
                throw;
            }
        }

        private async Task<bool> CustomerExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(e => e.CustomerId == id);
        }

    }
}
