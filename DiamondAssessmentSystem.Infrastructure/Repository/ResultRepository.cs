using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class ResultRepository : IResultRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public ResultRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Result>> GetResultsAsync()
        {
            return await _context.Results.Include(r => r.Request)
                                         .Include(r => r.Certificates)
                                         .ToListAsync();
        }

        public async Task<IEnumerable<Result>> GetResultsAsync(int customerId)
        {
            return await _context.Results
              .Include(r => r.Request)
              .Include(r => r.Certificates)
              .Where(r => r.Request.CustomerId == customerId)
              .ToListAsync();
        }

        public async Task<IEnumerable<Result>> GetPersonalResults(string userId)
        {
            var customerId = await GetCustomerId(userId);

            if (customerId == -1)
            {
                return Enumerable.Empty<Result>();
            }

            return await _context.Results
              .Include(r => r.Request).ThenInclude(r => r.Employee)
              .Include(r => r.Certificates)
              .Where(r => r.Request.CustomerId == customerId)  
              .ToListAsync();
        }

        public async Task<Result?> GetResultByIdAsync(int id)
        {
            return await _context.Results.Include(r => r.Request)   
                                         .ThenInclude(r => r.Employee) 
                                         .Include(r => r.Certificates)
                                         .FirstOrDefaultAsync(r => r.ResultId == id);
        }

        public async Task<Result> CreateResultAsync(Result result)
        {
            _context.Results.Add(result);

            try
            {
                await _context.SaveChangesAsync();
                return result;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        public async Task<bool> UpdateResultAsync(Result result)
        {
            _context.Entry(result).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ResultExistsAsync(result.ResultId))
                {
                    return false;
                }
                throw;
            }
        }

        private async Task<bool> ResultExistsAsync(int id)
        {
            return await _context.Results.AnyAsync(e => e.ResultId == id);
        }

        private async Task<int> GetCustomerId(string userId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);

            if (customer == null)
            {
                return -1;
            }

            return customer.CustomerId;
        }
    }
}
