using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class ServicePrice
{
    public int ServiceId { get; set; }

    public string ServiceType { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Duration { get; set; }

    public DateTime? DateCreated { get; set; }

    public int EmployeeId { get; set; }

    public string Status { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<ServicePriceAudit> ServicePriceAudits { get; set; } = new List<ServicePriceAudit>();
}
