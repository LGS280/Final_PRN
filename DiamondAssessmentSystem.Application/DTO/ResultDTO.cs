namespace DiamondAssessmentSystem.Application.DTO
{
    public class ResultDto
    {
        public int ResultId { get; set; }

        public int AssessmentStaff { get; set; }

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
        //public int? DiamondId { get; set; }

        public int RequestId { get; set; }

        public int AssessmentStaff { get; set; }

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
