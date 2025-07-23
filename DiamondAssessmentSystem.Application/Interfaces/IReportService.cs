using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportRequestStatusDto>> GetRequestCountsByStatusAsync();
        Task<int> GetTotalCustomerCountAsync();
        Task<int> GetTotalCertificatesIssuedAsync();
        Task<int> GetTotalProcessingRequestsAsync();
        Task<int> GetTotalEmployeesAsync();
        Task<int> GetTotalUsersByRoleAsync(string role);
        Task<int> GetTotalRequestsAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<List<ReportTopCustomerDto>> GetTopCustomersAsync(int top);
        Task<int> GetAccountCreatedInMonthAsync(int month);
        Task<int> GetTotalOrderCountAsync();
        Task<Dictionary<string, int>> GetOrderCountByTypeAsync();
        Task<Dictionary<string, int>> GetRequestChosenByUsersAsync();
    }
}
