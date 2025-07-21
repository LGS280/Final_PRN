using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class ConversationsController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var conversations = await _conversationService.GetMyAssignedConversationsAsync();
            return View(conversations); 
        }

        [HttpGet]
        public async Task<IActionResult> Unassigned()
        {
            var conversations = await _conversationService.GetUnassignedConversationsAsync();
            return View(conversations); 
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int id)
        {
            await _conversationService.AssignEmployeeAsync(id);
            return RedirectToAction(nameof(Unassigned));
        }

        [HttpGet]
        public async Task<IActionResult> StartOrGet()
        {
            var conversation = await _conversationService.StartOrGetConversationAsync();
            return RedirectToAction("Index", "ChatMessages", new { conversationId = conversation.ConversationId });
        }


        [HttpGet]
        public async Task<IActionResult> CustomerHome()
        {
            try
            {
                var conversation = await _conversationService.StartOrGetConversationAsync();
                return RedirectToAction("Index", "ChatMessages", new { conversationId = conversation.ConversationId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to start chat: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
