using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetOpenConversationByCustomerAsync(int customerId);
        Task<List<Conversation>> GetUnassignedConversationsAsync();
        Task<Conversation?> GetByIdAsync(int conversationId);
        Task AddAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
        Task<List<Conversation>> GetConversationsByEmployeeIdAsync(int employeeId);
    }
}
