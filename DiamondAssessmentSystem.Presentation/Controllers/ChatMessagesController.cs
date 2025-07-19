using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Presentation.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class ChatMessagesController : Controller
    {
        private readonly IChatMessageService _chatMessageService;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatMessagesController(
            IChatMessageService chatMessageService,
            IWebHostEnvironment env,
            IHubContext<ChatHub> hubContext)
        {
            _chatMessageService = chatMessageService;
            _env = env;
            _hubContext = hubContext;
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

            await _hubContext
                .Clients
                .Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", chatLog);

            return RedirectToAction(nameof(Index), new { conversationId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(int conversationId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            var chatLog = await _chatMessageService.UploadFileAsync(conversationId, file, _env.WebRootPath);

            await _hubContext
                .Clients
                .Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", chatLog);

            return RedirectToAction(nameof(Index), new { conversationId });
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
    }
}
