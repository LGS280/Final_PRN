using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class CommitmentRecord
{
    public int RecordId { get; set; }

    public int RequestId { get; set; }

    public DateOnly CommitDate { get; set; }

    public string? CommitmentReason { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Employee? ApprovedByNavigation { get; set; }

    public virtual Request Request { get; set; } = null!;
}
