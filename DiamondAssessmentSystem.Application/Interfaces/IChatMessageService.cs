using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IChatMessageService
    {
        Task<ChatLog> SendMessageAsync(int conversationId, CreateMessageDTO dto);
        Task<List<ChatLogDTO>> GetChatHistoryAsync(int conversationId);
        Task<ChatLogDTO> UploadFileAsync(int conversationId, IFormFile file, string rootPath);
        Task<(string physicalPath, string fileName)?> GetDownloadFileInfoAsync(
            int conversationId, int? chatId, string? savedFileName, string webRootPath);
        Task<ChatLogDTO?> GetChatLogByIdAsync(int chatId);
    }
}
