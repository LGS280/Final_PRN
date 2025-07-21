using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize(Roles = "Consultant")]
    public class ConsultantController : Controller
    {
        private readonly IConversationService _conversationService;

        public ConsultantController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var unassigned = await _conversationService.GetUnassignedConversationsAsync();
                var assigned = await _conversationService.GetMyAssignedConversationsAsync();

                var model = (Unassigned: unassigned, Assigned: assigned, Error: (string?)null);
                return View(model);
            }
            catch (Exception ex)
            {
                var model = (Unassigned: new List<ConversationDTO>(), Assigned: new List<ConversationDTO>(), Error: ex.Message);
                return View(model);
            }
        }
    }
}
