using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IChatLogRepository
    {
        Task<List<ChatLog>> GetByConversationIdAsync(int conversationId);
        Task<ChatLog> AddAsync(ChatLog chatLog);
        Task<ChatLog?> GetByIdAsync(int chatId);
    }
}
