using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public ReportRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsersByRoleAsync(string role)
        {
            return await (from user in _context.Users
                          join userRole in _context.UserRoles on user.Id equals userRole.UserId
                          join roleEntity in _context.Roles on userRole.RoleId equals roleEntity.Id
                          where roleEntity.Name == role
                          select user).CountAsync();
        }

        public async Task<int> GetTotalRequestsAsync()
        {
            return await _context.Requests.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Requests
                .Where(r => r.Status == "Completed")
                .Include(r => r.Service)
                .SumAsync(r => (decimal?)r.Service.Price) ?? 0;
        }

        public async Task<IEnumerable<(string CustomerName, int RequestCount)>> GetTopCustomersAsync(int top)
        {
            var result = await _context.Requests
                .GroupBy(r => r.CustomerId)
                .OrderByDescending(g => g.Count())
                .Take(top)
                .Select(g => new
                {
                    CustomerName = g.First().Customer.UnitName,
                    RequestCount = g.Count()
                })
                .ToListAsync();

            return result
                .Select(x => (x.CustomerName, x.RequestCount))
                .ToList();
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            return await GetTotalUsersByRoleAsync("Employee");
        }

        public async Task<int> GetTotalProcessingRequestsAsync()
        {
            return await _context.Requests.CountAsync(r => r.Status == "Processing");
        }

        public async Task<int> GetTotalCertificatesIssuedAsync()
        {
            return await _context.Certificates.CountAsync();
        }

        public async Task<int> GetTotalCustomerCountAsync()
        {
            return await _context.Customers.CountAsync();
        }


        public async Task<List<(string Status, int RequestCount)>> GetRequestCountsByStatusAsync()
        {
            var data = await _context.Requests
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, RequestCount = g.Count() })
                .OrderByDescending(g => g.RequestCount)
                .ToListAsync(); // EF async tới đây

            // Sau đó chuyển sang tuple trong bộ nhớ
            return data
                .Select(g => (g.Status, g.RequestCount))
                .ToList();
        }

        public async Task<int> GetAccountCreatedInMonthAsync(int month)
        {
            return await _context.Users
                .Where(a => a.DateCreated.Value.Month == month && a.DateCreated.Value.Year == DateTime.Now.Year)
                .CountAsync();
        }

        public async Task<int> GetTotalOrderCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetOrderCountByTypeAsync()
        {
            return await _context.Orders
                .GroupBy(o => o.Service.ServiceType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetRequestChosenByUsersAsync()
        {
            return await _context.Requests
                .GroupBy(r => r.RequestType ?? "Unknown")
                .Select(g => new { RequestType = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RequestType, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetAccountCreatedPerDayAsync(DateTime from, DateTime to)
        {
            return await _context.Users
                .Where(a => a.DateCreated.Value.Date >= from.Date && a.DateCreated.Value.Date <= to.Date)
                .GroupBy(a => a.DateCreated.Value.Date)
                .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
                .ToDictionaryAsync(x => x.Date, x => x.Count);
        }

        public async Task<int> GetTotalRequestChosenAsync()
        {
            return await _context.Requests.CountAsync();
        }

        public async Task<Dictionary<string, int>> GetOrderCountByStatusAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);
        }
        public async Task<Dictionary<string, int>> GetRequestCountByStatusAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Requests
                .Where(r => r.RequestDate >= fromDate && r.RequestDate <= toDate)
                .GroupBy(r => r.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Status, g => g.Count);
        }

        public async Task<int> GetTotalOrderCountAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetOrderCountByTypeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate)
                .GroupBy(o => o.Status)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.Type, g => g.Count);
        }

        public async Task<int> GetTotalRequestChosenAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Requests
                .Where(r => r.RequestDate >= fromDate && r.RequestDate <= toDate)
                .CountAsync();
        }


    }
}
