using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public ConversationService(
            IConversationRepository conversationRepo,
            IMapper mapper,
            ICurrentUserService currentUser)
        {
            _conversationRepo = conversationRepo;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<ConversationDTO> StartOrGetConversationAsync()
        {
            EnsureCustomerOrConsultant();

            var existing = await _conversationRepo.GetOpenConversationByCustomerAsync((int)_currentUser.AssociatedId);
            if (existing != null)
            {
                return _mapper.Map<ConversationDTO>(existing);
            }

            var newConv = new Conversation
            {
                CustomerId = (int)_currentUser.AssociatedId,
                Status = "open",
                CreatedAt = DateTime.UtcNow
            };

            await _conversationRepo.AddAsync(newConv);
            return _mapper.Map<ConversationDTO>(newConv);
        }

        public async Task AssignEmployeeAsync(int conversationId)
        {
            EnsureConsultant();
            ThrowIfInvalidId(conversationId, nameof(conversationId));

            var conversation = await _conversationRepo.GetByIdAsync(conversationId)
                ?? throw new InvalidOperationException("Conversation not found.");

            if (conversation.EmployeeId != null)
                throw new InvalidOperationException("Conversation has already been assigned.");

            conversation.EmployeeId = _currentUser.AssociatedId;
            await _conversationRepo.UpdateAsync(conversation);
        }

        public async Task<List<ConversationDTO>> GetUnassignedConversationsAsync()
        {
            EnsureConsultant();

            var list = await _conversationRepo.GetUnassignedConversationsAsync();
            return _mapper.Map<List<ConversationDTO>>(list);
        }

        public async Task<List<ConversationDTO>> GetMyAssignedConversationsAsync()
        {
            EnsureConsultant();

            var list = await _conversationRepo.GetConversationsByEmployeeIdAsync((int)_currentUser.AssociatedId);
            return _mapper.Map<List<ConversationDTO>>(list);
        }

        // Helpers
        private void EnsureConsultant()
        {
            if (_currentUser.Role != "Consultant")
                throw new UnauthorizedAccessException("Only consultants can perform this action.");
        }

        private void EnsureCustomerOrConsultant()
        {
            if (_currentUser.Role != "Consultant" && _currentUser.Role != "Customer")
                throw new UnauthorizedAccessException("Only customers or consultants can perform this action.");
        }

        private void ThrowIfInvalidId(int id, string paramName)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0.", paramName);
        }
    }
}
