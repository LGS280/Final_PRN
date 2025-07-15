using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IConversationService
    {
        Task<ConversationDTO> StartOrGetConversationAsync();
        Task AssignEmployeeAsync(int conversationId);
        Task<List<ConversationDTO>> GetUnassignedConversationsAsync();
        Task<List<ConversationDTO>> GetMyAssignedConversationsAsync();
    }
}
