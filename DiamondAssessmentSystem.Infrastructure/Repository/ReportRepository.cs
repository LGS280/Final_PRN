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
                        .Where(r => r.Status == "Done")
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
    }
}
