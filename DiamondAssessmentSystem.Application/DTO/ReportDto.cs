using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ReportDto
    {
    }

    public class ReportTopCustomerDto
    {
        public string CustomerName { get; set; }
        public int RequestCount { get; set; }
    }

    public class ReportRequestStatusDto
    {
        public string Status { get; set; }
        public int RequestCount { get; set; }
    }

    public class ReportSummaryDto
    {
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalRequests { get; set; }
        public int TotalProcessingRequests { get; set; }
        public int TotalCertificatesIssued { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ReportRequestStatusDto> RequestCountsByStatus { get; set; }
        public List<ReportTopCustomerDto> TopCustomers { get; set; }
    }
}
