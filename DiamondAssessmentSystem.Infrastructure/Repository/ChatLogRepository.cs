using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class ChatLogRepository : IChatLogRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public ChatLogRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<ChatLog?> GetByIdAsync(int chatId)
        {
            return await _context.ChatLogs.FirstOrDefaultAsync(c => c.ChatId == chatId);
        }

        public async Task<List<ChatLog>> GetByConversationIdAsync(int conversationId)
        {
            return await _context.ChatLogs
                .Where(c => c.ConversationId == conversationId)
                .OrderBy(c => c.SentAt)
                .ToListAsync();
        }

        public async Task<ChatLog?> AddAsync(ChatLog chatLog)
        {
            var chatEntry = _context.ChatLogs.Add(chatLog);
            int result = await _context.SaveChangesAsync();

            return result > 0 ? chatEntry.Entity : null;
        }
    }
}
