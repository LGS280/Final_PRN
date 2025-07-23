using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<int> GetTotalUsersByRoleAsync(string role)
            => await _reportRepository.GetTotalUsersByRoleAsync(role);

        public async Task<int> GetTotalRequestsAsync()
            => await _reportRepository.GetTotalRequestsAsync();

        public async Task<decimal> GetTotalRevenueAsync()
            => await _reportRepository.GetTotalRevenueAsync();

        public async Task<List<ReportTopCustomerDto>> GetTopCustomersAsync(int top)
        {
            var result = await _reportRepository.GetTopCustomersAsync(top);
            return _mapper.Map<List<ReportTopCustomerDto>>(result);
        }

        public async Task<int> GetTotalEmployeesAsync()
            => await _reportRepository.GetTotalEmployeesAsync();

        public async Task<int> GetTotalProcessingRequestsAsync()
            => await _reportRepository.GetTotalProcessingRequestsAsync();

        public async Task<int> GetTotalCertificatesIssuedAsync()
            => await _reportRepository.GetTotalCertificatesIssuedAsync();

        public async Task<int> GetTotalCustomerCountAsync()
            => await _reportRepository.GetTotalCustomerCountAsync();

        public async Task<List<ReportRequestStatusDto>> GetRequestCountsByStatusAsync()
        {
            var result = await _reportRepository.GetRequestCountsByStatusAsync();
            return _mapper.Map<List<ReportRequestStatusDto>>(result);
        }

        public async Task<int> GetAccountCreatedInMonthAsync(int month)
        => await _reportRepository.GetAccountCreatedInMonthAsync(month);

        public async Task<int> GetTotalOrderCountAsync()
            => await _reportRepository.GetTotalOrderCountAsync();

        public async Task<Dictionary<string, int>> GetOrderCountByTypeAsync()
            => await _reportRepository.GetOrderCountByTypeAsync();

        public async Task<Dictionary<string, int>> GetRequestChosenByUsersAsync()
            => await _reportRepository.GetRequestChosenByUsersAsync();

        public async Task<int> GetTotalRequestChosenAsync() 
            => await _reportRepository.GetTotalRequestChosenAsync();
        public async Task<Dictionary<string, int>> GetAccountCreatedPerDayAsync(DateTime from, DateTime to)
            => await _reportRepository.GetAccountCreatedPerDayAsync(from, to);
    }

}
