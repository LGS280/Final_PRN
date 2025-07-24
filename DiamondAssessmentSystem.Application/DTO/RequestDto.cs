
using DiamondAssessmentSystem.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class RequestDto
    {
        public int RequestId { get; set; }
        public string FormType { get; set; }
        public DateTime RequestDate { get; set; }
        public int ServiceId { get; set; }
        public string? RequestType { get; set; }
        public string? Status { get; set; }

        public string? ServiceType { get; set; }
        public decimal? ServicePrice { get; set; }
        public int? ServiceDuration { get; set; }
        public string? ServiceDescription { get; set; }

        public string? EmployeeName { get; set; }
    }

    public class RequestCreateDto
    {
        [Required]
        public int ServiceId { get; set; }

        [MaxLength(50)]
        public string? RequestType { get; set; }

        public int? EmployeeId { get; set; }
    }

    public class RequestWithServiceDto
    {
        public int RequestId { get; set; }
        public string? RequestType { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Status { get; set; }

        public int ServiceId { get; set; }
        public string ServiceType { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }

        public string? EmployeeName { get; set; } 
    }

    public class RequestStatisticDto
    {
        public int TotalPending { get; set; }
        public int TotalCancelled { get; set; }
    }
}
