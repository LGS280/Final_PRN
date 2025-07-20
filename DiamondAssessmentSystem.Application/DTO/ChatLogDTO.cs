using DiamondAssessmentSystem.Infrastructure.Enums;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class ChatLogDTO
    {
        public int ChatId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderRole { get; set; } 
        public MessageType MessageType { get; set; }
        public string? Message { get; set; }
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? SavedFileName { get; set; }
        public int? FileSize { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}