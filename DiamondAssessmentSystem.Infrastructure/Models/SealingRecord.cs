using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class SealingRecord
{
    public int SealingId { get; set; }

    public int RequestId { get; set; }

    public DateTime SealDate { get; set; }

    public string? SealingReason { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Employee? ApprovedByNavigation { get; set; }

    public virtual Request Request { get; set; } = null!;
}
