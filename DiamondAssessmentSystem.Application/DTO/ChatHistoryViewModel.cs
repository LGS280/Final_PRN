namespace DiamondAssessmentSystem.Application.DTO
{
    public class ChatHistoryViewModel
    {
        public int ConversationId { get; set; }
        public List<ChatLogDTO> Messages { get; set; } = new();
    }
}