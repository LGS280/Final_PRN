
using DiamondAssessmentSystem.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class RequestDto
    {
        public int RequestId { get; set; }
        public string FormType { get; set; }
        public DateTime RequestDate { get; set; }
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public string? RequestType { get; set; }
        public int? EmployeeId { get; set; }
        public string? Status { get; set; }
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
        public string RequestType { get; set; }
        public DateTime RequestDate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceType { get; set; }
        public decimal Price { get; set; }
    }
}
