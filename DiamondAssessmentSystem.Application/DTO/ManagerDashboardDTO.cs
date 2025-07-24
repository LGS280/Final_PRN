using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ManagerDashboardDTO
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public Dictionary<string, int> AccountsCreatedPerDay { get; set; }
        public int TotalOrders { get; set; }
        public Dictionary<string, int> OrdersByType { get; set; }
        public Dictionary<string, int> OrderStatusStats { get; set; } = new();// Paid vs Cancelled
        public Dictionary<string, int> RequestStatusStats { get; set; } // Pending, Completed, Cancelled

        public int TotalRequestChosen { get; set; }
    }

}
