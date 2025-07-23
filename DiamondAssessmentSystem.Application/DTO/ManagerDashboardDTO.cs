using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ManagerDashboardDTO
    {
        public int AccountsCreatedThisMonth { get; set; }
        public int TotalOrders { get; set; }
        public Dictionary<string, int> OrdersByType { get; set; } = new();
        public Dictionary<string, int> RequestsChosen { get; set; } = new();
    }
}
