using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiamondAssessmentSystem.Controllers
{
    [ApiController]
    [Route("api/conversations")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpPost("start")]
        public async Task<ActionResult<ConversationDTO>> StartOrGetConversation()
        {
            var conversation = await _conversationService.StartOrGetConversationAsync();
            return Ok(conversation);
        }

        [HttpPost("{id:int}/assign")]
        public async Task<IActionResult> AssignConversation(int id)
        {
            await _conversationService.AssignEmployeeAsync(id);
            return NoContent();
        }

        [HttpGet("unassigned")]
        public async Task<ActionResult<List<ConversationDTO>>> GetUnassigned()
        {
            var conversations = await _conversationService.GetUnassignedConversationsAsync();
            return Ok(conversations);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<ConversationDTO>>> GetMyAssigned()
        {
            var conversations = await _conversationService.GetMyAssignedConversationsAsync();
            return Ok(conversations);
        }
    }
}
