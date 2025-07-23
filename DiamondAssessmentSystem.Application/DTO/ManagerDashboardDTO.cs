using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ManagerDashboardDTO
    {
        public Dictionary<string, int> AccountsCreatedPerDay { get; set; } = new();
        public int TotalOrders { get; set; }
        public Dictionary<string, int> OrdersByType { get; set; } = new();
        public int TotalRequestChosen { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
