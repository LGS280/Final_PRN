using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string UserId { get; set; }

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public decimal? Salary { get; set; }
        public string? Status { get; set; } 
    }

    public class EmployeeUpdateDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public decimal? Salary { get; set; }
    }
}
