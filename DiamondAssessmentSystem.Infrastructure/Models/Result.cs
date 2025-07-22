using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class Result
{
    public int ResultId { get; set; }

    public int DiamondId { get; set; }

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

    public DateTime? ModifiedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual Request Request { get; set; } = null!;
}
