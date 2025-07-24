using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class ServicePriceAudit
{
    public int AuditId { get; set; }

    public int? ServiceId { get; set; }

    public string? ServiceType { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal? NewPrice { get; set; }

    public int? OldDuration { get; set; }

    public int? NewDuration { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime ChangeDate { get; set; }

    public string ActionType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual ServicePrice? Service { get; set; }
}
