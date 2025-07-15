using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public ConversationRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetOpenConversationByCustomerAsync(int customerId)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.Status == "open");
        }

        public async Task<List<Conversation>> GetUnassignedConversationsAsync()
        {
            return await _context.Conversations
                .Where(c => c.EmployeeId == null && c.Status == "open")
                .Include(c => c.Customer)
                .ToListAsync();
        }

        public async Task<Conversation?> GetByIdAsync(int conversationId)
        {
            return await _context.Conversations
                .Include(c => c.ChatLogs)
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
        }

        public async Task AddAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Conversation conversation)
        {
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Conversation>> GetConversationsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Conversations
                .Where(c => c.EmployeeId == employeeId)
                .Include(c => c.Customer)
                .ToListAsync();
        }
    }
}
