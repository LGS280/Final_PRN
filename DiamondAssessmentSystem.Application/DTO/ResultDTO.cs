using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ResultDto
    {
        public int ResultId { get; set; }

        public int? DiamondId { get; set; }

        public int RequestId { get; set; }

        public string DiamondOrigin { get; set; } = null!;

        public string Shape { get; set; } = null!;

        public string Measurements { get; set; } = null!;

        public decimal CaratWeight { get; set; }

        public string Color { get; set; } = null!;

        public string Clarity { get; set; } = null!;

        public string Cut { get; set; } = null!;

        public string? Proportions { get; set; }

        public string? Polish { get; set; }

        public string? Symmetry { get; set; }

        public string? Fluorescence { get; set; }

        public string Status { get; set; } = null!;
    }

    public class ResultCreateDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Diamond Origin is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces allowed.")]
        public string DiamondOrigin { get; set; } = null!;

        [Required(ErrorMessage = "Shape is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces allowed.")]
        public string Shape { get; set; } = null!;

        [Required(ErrorMessage = "Measurements are required.")]
        [StringLength(100, ErrorMessage = "Measurements too long.")]
        public string Measurements { get; set; } = null!;

        [Range(0.01, 100.0, ErrorMessage = "Carat Weight must be greater than 0.")]
        public decimal CaratWeight { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces allowed.")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Clarity is required.")]
        [RegularExpression(@"^[A-Za-z0-9\s]+$", ErrorMessage = "Only letters, numbers and spaces allowed.")]
        public string Clarity { get; set; } = null!;

        [Required(ErrorMessage = "Cut is required.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces allowed.")]
        public string Cut { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Proportions too long.")]
        public string? Proportions { get; set; }

        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "Only letters and spaces allowed.")]
        public string? Polish { get; set; }

        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "Only letters and spaces allowed.")]
        public string? Symmetry { get; set; }

        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "Only letters and spaces allowed.")]
        public string? Fluorescence { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Pending|Completed|Rejected)$", ErrorMessage = "Invalid status.")]
        public string Status { get; set; } = null!;
    }

    public class ResultUpdateDto
    {
        public int ResultId { get; set; }
        public int RequestId { get; set; }
        public string? DiamondOrigin { get; set; }
        public string? Shape { get; set; }
        public string? Measurements { get; set; }
        public decimal? CaratWeight { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Cut { get; set; }
        public string? Proportions { get; set; }
        public string? Polish { get; set; }
        public string? Symmetry { get; set; }
        public string? Fluorescence { get; set; }
        public string? Status { get; set; }
    }

}
