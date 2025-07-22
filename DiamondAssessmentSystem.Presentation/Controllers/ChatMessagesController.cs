using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.Enums;
using DiamondAssessmentSystem.Presentation.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class ChatMessagesController : Controller
    {
        private readonly IChatMessageService _chatMessageService;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatMessagesController> _logger;

        public ChatMessagesController(
            IChatMessageService chatMessageService,
            IWebHostEnvironment env,
            IHubContext<ChatHub> hubContext,
            ILogger<ChatMessagesController> logger)
        {
            _chatMessageService = chatMessageService;
            _env = env;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int conversationId)
        {
            if (conversationId <= 0)
                return BadRequest("Invalid conversation ID.");

            var messages = await _chatMessageService.GetChatHistoryAsync(conversationId);

            var model = new ChatHistoryViewModel
            {
                ConversationId = conversationId,
                Messages = messages
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int conversationId, CreateMessageDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var chatLog = await _chatMessageService.SendMessageAsync(conversationId, dto);

            var messageDto = new MessageResponseDTO
            {
                ChatId = chatLog.ChatId,
                ConversationId = chatLog.ConversationId,
                SenderId = chatLog.SenderId,
                SenderName = chatLog.SenderName,
                SenderRole = chatLog.SenderRole?.ToLowerInvariant(),
                MessageType = chatLog.MessageType,
                Message = chatLog.Message,
                FilePath = chatLog.FilePath,
                FileName = chatLog.FileName,
                FileSize = chatLog.FileSize,
                SentAt = chatLog.SentAt
            };

            await _hubContext.Clients
                .Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", messageDto);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(int conversationId, IFormFile file)
        {
            Debug.WriteLine($"UploadFile invoked. conversationId={conversationId}");

            if (file == null)
            {
                Debug.WriteLine("File is null.");
            }
            else
            {
                Debug.WriteLine($"File info: Name={file.FileName}, Length={file.Length}");
            }

            if (file == null || file.Length == 0)
            {
                Debug.WriteLine("File is missing or empty.");
                return BadRequest("File is required.");
            }

            try
            {
                var chatLog = await _chatMessageService.UploadFileAsync(conversationId, file, _env.WebRootPath);

                var messageDto = new MessageResponseDTO
                {
                    ChatId = chatLog.ChatId,
                    ConversationId = chatLog.ConversationId,
                    SenderId = chatLog.SenderId,
                    SenderName = chatLog.SenderName,
                    SenderRole = chatLog.SenderRole,
                    MessageType = chatLog.MessageType,
                    Message = chatLog.Message,
                    FilePath = chatLog.FilePath,
                    FileName = chatLog.FileName,
                    FileSize = chatLog.FileSize,
                    SentAt = chatLog.SentAt
                };

                Debug.WriteLine("File uploaded and chat log saved successfully.");

                await _hubContext.Clients
                    .Group(conversationId.ToString())
                    .SendAsync("ReceiveMessage", messageDto);

                return Ok(messageDto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex, "Error during file upload.");
                return StatusCode(500, "Internal error.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Download(int conversationId, int? chatId, string? savedFileName)
        {
            var result = await _chatMessageService.GetDownloadFileInfoAsync(
                conversationId, chatId, savedFileName, _env.WebRootPath);

            if (result == null)
                return NotFound();

            var (filePath, fileName) = result.Value;
            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendGeminiMessage(int conversationId, [FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message) || conversationId <= 0)
                return BadRequest("Invalid input");

            var dto = new CreateMessageDTO
            {
                SenderRole = "consultant",
                SenderName = "Gemini",
                MessageType = MessageType.Text,
                Message = message
            };

            var chatLog = await _chatMessageService.SendMessageAsync(conversationId, dto);

            var messageDto = new MessageResponseDTO
            {
                ChatId = chatLog.ChatId,
                ConversationId = chatLog.ConversationId,
                SenderId = chatLog.SenderId,
                SenderName = chatLog.SenderName,
                SenderRole = chatLog.SenderRole,
                MessageType = chatLog.MessageType,
                Message = chatLog.Message,
                FilePath = chatLog.FilePath,
                FileName = chatLog.FileName,
                FileSize = chatLog.FileSize,
                SentAt = chatLog.SentAt
            };

            return Ok(messageDto);
        }
    }
}
