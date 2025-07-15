using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.Helpers;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IConversationRepository _conversationRepo;
        private readonly IChatLogRepository _chatLogRepo;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<ChatMessageService> _logger;

        public ChatMessageService(
            IConversationRepository conversationRepo,
            IChatLogRepository chatLogRepo,
            IMapper mapper,
            ICurrentUserService currentUser,
            ILogger<ChatMessageService> logger)
        {
            _conversationRepo = conversationRepo;
            _chatLogRepo = chatLogRepo;
            _mapper = mapper;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ChatLog> SendMessageAsync(int conversationId, CreateMessageDTO DTO)
        {
            ThrowIfInvalidId(conversationId, nameof(conversationId));
            _ = DTO ?? throw new ArgumentNullException(nameof(DTO));

            await EnsureUserCanAccessAndIsOpen(conversationId);

            DTO.SenderId = (int)_currentUser.AssociatedId;
            DTO.SenderName = _currentUser.UserName;
            DTO.SenderRole = _currentUser.Role;

            var chatLog = _mapper.Map<ChatLog>(DTO);
            chatLog.ConversationId = conversationId;
            chatLog.SentAt = DateTime.UtcNow;

            return await _chatLogRepo.AddAsync(chatLog);
        }

        public async Task<List<ChatLogDTO>> GetChatHistoryAsync(int conversationId)
        {
            ThrowIfInvalidId(conversationId, nameof(conversationId));
            await EnsureUserCanAccessAndIsOpen(conversationId);

            var logs = await _chatLogRepo.GetByConversationIdAsync(conversationId);
            return _mapper.Map<List<ChatLogDTO>>(logs);
        }

        public async Task<ChatLogDTO> UploadFileAsync(int conversationId, IFormFile file, string rootPath)
        {
            ThrowIfInvalidId(conversationId, nameof(conversationId));
            _ = file ?? throw new ArgumentNullException(nameof(file));
            if (string.IsNullOrWhiteSpace(rootPath))
                throw new ArgumentException("Root path must not be empty.", nameof(rootPath));

            await EnsureUserCanAccessAndIsOpen(conversationId);

            var (savedFileName, relativePath) = await SaveFileAsync(file, conversationId, rootPath);

            var createMessageDTO = BuildFileMessageDTO(file, savedFileName, relativePath);
            var chatLog = await SendMessageAsync(conversationId, createMessageDTO);

            return _mapper.Map<ChatLogDTO>(chatLog);
        }

        public async Task<(string physicalPath, string fileName)?> GetDownloadFileInfoAsync(
            int conversationId, int? chatId, string? savedFileName, string webRootPath)
        {
            ThrowIfInvalidId(conversationId, nameof(conversationId));
            if (string.IsNullOrWhiteSpace(webRootPath))
                throw new ArgumentException("Web root path must not be empty.", nameof(webRootPath));

            ChatLogDTO? log = null;

            if (chatId.HasValue)
            {
                log = await GetChatLogByIdAsync(chatId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(savedFileName))
            {
                var logs = await GetChatHistoryAsync(conversationId);
                log = logs.FirstOrDefault(m =>
                    !string.IsNullOrWhiteSpace(m.SavedFileName) &&
                    m.SavedFileName.Equals(savedFileName, StringComparison.OrdinalIgnoreCase));
            }

            if (log == null || string.IsNullOrWhiteSpace(log.SavedFileName))
                return null;

            var uploadsFolder = Path.Combine(webRootPath, "uploads", "chat", conversationId.ToString());
            var filePath = Path.Combine(uploadsFolder, log.SavedFileName);

            return File.Exists(filePath) ? (filePath, log.FileName) : null;
        }

        public async Task<ChatLogDTO?> GetChatLogByIdAsync(int chatId)
        {
            ThrowIfInvalidId(chatId, nameof(chatId));

            var chat = await _chatLogRepo.GetByIdAsync(chatId);
            return _mapper.Map<ChatLogDTO>(chat);
        }

        // Helpers
        private async Task<Conversation> EnsureUserCanAccessAndIsOpen(int conversationId)
        {
            var conversation = await _conversationRepo.GetByIdAsync(conversationId)
                ?? throw new InvalidOperationException("Conversation not found.");

            if (!conversation.Status.Equals("open", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Conversation is closed.");

            if (_currentUser.Role == "Customer" && conversation.CustomerId != _currentUser.AssociatedId)
                throw new UnauthorizedAccessException("You do not have access to this conversation.");

            if (_currentUser.Role == "Consultant" && conversation.EmployeeId != _currentUser.AssociatedId)
                throw new UnauthorizedAccessException("You do not have access to this conversation.");

            return conversation;
        }

        private async Task<(string savedFileName, string relativePath)> SaveFileAsync(IFormFile file, int conversationId, string rootPath)
        {
            var originalFileName = Path.GetFileName(file.FileName);
            var ext = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!FileHelper.IsAllowed(originalFileName))
                throw new InvalidOperationException($"File type not allowed: {ext}");

            var uniqueName = FileHelper.GenerateUniqueFileName(ext);
            var uploadsFolder = Path.Combine(rootPath, "uploads", "chat", conversationId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var savePath = Path.Combine(uploadsFolder, uniqueName);
            var relativePath = $"/uploads/chat/{conversationId}/{uniqueName}";

            await using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("File saved to {Path}", savePath);

            return (uniqueName, relativePath);
        }

        private CreateMessageDTO BuildFileMessageDTO(IFormFile file, string savedFileName, string relativePath)
        {
            return new CreateMessageDTO
            {
                SenderId = (int)_currentUser.AssociatedId,
                SenderName = _currentUser.UserName,
                SenderRole = _currentUser.Role,
                MessageType = FileHelper.GetMessageType(file.FileName),
                FileName = file.FileName,
                SavedFileName = savedFileName,
                FilePath = relativePath,
                FileSize = (int)file.Length
            };
        }

        private void ThrowIfInvalidId(int id, string paramName)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be greater than 0.", paramName);
        }
    }
}
