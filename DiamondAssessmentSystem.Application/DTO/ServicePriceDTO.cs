using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ServicePriceDto
    {
        public int ServiceId { get; set; }

        [Required]
        public string ServiceType { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Duration { get; set; }

        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        [Required]
        public string Status { get; set; } = null!;
    }

    public class ServicePriceCreateDto
    {
        [Required]
        public string ServiceType { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int Duration { get; set; }

        public int EmployeeId { get; set; }

        [Required]
        public string Status { get; set; } = null!;
    }
}
