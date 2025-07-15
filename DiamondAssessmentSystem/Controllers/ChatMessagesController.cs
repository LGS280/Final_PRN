using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DiamondAssessmentSystem.Controllers
{
    [ApiController]
    [Route("api/conversations/{conversationId:int}/messages")]
    public class ChatMessagesController : ControllerBase
    {
        private readonly IChatMessageService _chatMessageService;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatMessagesController(
            IChatMessageService chatMessageService,
            IWebHostEnvironment env,
            IMapper mapper,
            IHubContext<ChatHub> hubContext)
        {
            _chatMessageService = chatMessageService;
            _env = env;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatLogDTO>>> GetMessages(int conversationId)
        {
            var messages = await _chatMessageService.GetChatHistoryAsync(conversationId);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<MessageResponseDTO>> SendMessage(
            int conversationId,
            [FromBody] CreateMessageDTO dto)
        {
            var chatLog = await _chatMessageService.SendMessageAsync(conversationId, dto);
            var messageResponse = _mapper.Map<MessageResponseDTO>(chatLog);

            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", messageResponse);

            return Ok(messageResponse);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(30_000_000)]
        public async Task<ActionResult<MessageResponseDTO>> UploadFile(
            int conversationId,
            IFormFile file)
        {
            var chatLogDto = await _chatMessageService.UploadFileAsync(conversationId, file, _env.WebRootPath);
            var messageResponse = _mapper.Map<MessageResponseDTO>(chatLogDto);

            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", messageResponse);

            return Ok(messageResponse);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(
            int conversationId,
            [FromQuery] int? chatId,
            [FromQuery] string? savedFileName)
        {
            var result = await _chatMessageService.GetDownloadFileInfoAsync(conversationId, chatId, savedFileName, _env.WebRootPath);

            if (result == null)
                return NotFound();

            var (filePath, fileName) = result.Value;
            return PhysicalFile(filePath, "application/octet-stream", fileDownloadName: fileName);
        }
    }
}
