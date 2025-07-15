using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public RequestRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Request>> GetRequestsAsync()
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .Include(r => r.CommitmentRecords)
                .Include(r => r.SealingRecords)
                .ToListAsync();
        }

        public async Task<Request?> GetRequestByIdAsync(int id)
        {
            return await _context.Requests
                .Include(r => r.Customer)
                .Include(r => r.Employee)
                .Include(r => r.CommitmentRecords)
                .Include(r => r.SealingRecords)
                .FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task<bool> CreateDraftRequest(string userId, Request request)
        {
            var customerId = await GetCustomerId(userId);

            if (customerId == -1)
            {
                return false;
            }

            request.Status = "Draft";
            request.RequestDate = DateTime.Now;

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelRequestAsync(string userId, int requestId)
        {
            var customerId = await GetCustomerId(userId);

            if (customerId == -1)
            {
                return false;
            }

            var request = await _context.Requests.FirstOrDefaultAsync(r => r.CustomerId == customerId && r.RequestId == requestId);

            if (request == null || request.Status != "Draft")
                return false;

            request.Status = "Cancelled";
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Request>> GetRequestsByCustomerIdAsync(string userId)
        {
            var customerId = await GetCustomerId(userId);

            if (customerId == -1)
            {
                return Enumerable.Empty<Request>();
            }

            return await _context.Requests
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Service)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<Request> CreateRequestAsync(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> UpdateRequestAsync(Request request)
        {
            _context.Entry(request).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(request.RequestId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.RequestId == id);
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
