using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ConversationDTO
    {
        public int ConversationId { get; set; }
        public int CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public string Status { get; set; } = "open";
        public DateTime CreatedAt { get; set; }
    }
}
