using DiamondAssessmentSystem.Application.DTO;

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
        Task<Dictionary<string, int>> GetAccountCreatedPerDayAsync(DateTime from, DateTime to);
        Task<int> GetTotalRequestChosenAsync();
        Task<int> GetTotalOrderCountAsync(DateTime fromDate, DateTime toDate);
        Task<Dictionary<string, int>> GetOrderCountByTypeAsync(DateTime fromDate, DateTime toDate);
        Task<int> GetTotalRequestChosenAsync(DateTime fromDate, DateTime toDate);

        Task<Dictionary<string, int>> GetOrderStatusReportAsync(DateTime fromDate, DateTime toDate);
        Task<Dictionary<string, int>> GetRequestStatusReportAsync(DateTime fromDate, DateTime toDate);
    }
}
