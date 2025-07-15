using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public class Conversation
{
    public int ConversationId { get; set; }

    public int CustomerId { get; set; }
    public int? EmployeeId { get; set; }     

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Status { get; set; } = "open"; 

    public Customer Customer { get; set; } = null!;
    public Employee? Employee { get; set; }
    public ICollection<ChatLog> ChatLogs { get; set; } = new List<ChatLog>();
}

