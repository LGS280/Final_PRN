using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ChatHistoryViewModel
    {
        public int ConversationId { get; set; }
        public List<ChatLogDTO> Messages { get; set; } = new();
    }
}
