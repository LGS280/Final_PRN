using DiamondAssessmentSystem.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DiamondAssessmentSystem.Infrastructure.Models;

public class ChatLog
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

    public bool IsRead { get; set; } = false;
    public DateTime SentAt { get; set; } = DateTime.Now;

    public Conversation Conversation { get; set; } = null!;
}
