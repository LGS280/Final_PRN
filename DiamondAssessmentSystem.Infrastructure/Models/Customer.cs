using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public decimal? Idcard { get; set; }

    public string? Address { get; set; }

    public string? UnitName { get; set; }

    public string? TaxCode { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }

    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
