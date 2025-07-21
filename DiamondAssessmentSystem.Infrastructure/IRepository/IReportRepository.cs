using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IReportRepository
    {
        Task<int> GetTotalUsersByRoleAsync(string role);
        Task<int> GetTotalRequestsAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<IEnumerable<(string CustomerName, int RequestCount)>> GetTopCustomersAsync(int top);
        Task<int> GetTotalEmployeesAsync();
        Task<int> GetTotalProcessingRequestsAsync();
        Task<int> GetTotalCertificatesIssuedAsync();
        Task<int> GetTotalCustomerCountAsync();
        Task<List<(string Status, int RequestCount)>> GetRequestCountsByStatusAsync();
    }
}
