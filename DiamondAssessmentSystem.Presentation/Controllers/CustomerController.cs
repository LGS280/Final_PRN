using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization; // Required
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    //[Authorize]   Requires authentication
    public class CustomerController : Controller // MVC controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICurrentUserService _currentUser;
        private readonly IConversationService _conversationService;

        public CustomerController(ICustomerService customerService, ICurrentUserService currentUser, IConversationService conversationService)
        {
            _customerService = customerService;
            _currentUser = currentUser;
            _conversationService = conversationService;
        }

        [HttpGet]
        public async Task<IActionResult> Home()
        {
            var username = User.Identity?.Name ?? "Guest";
            var conversation = await _conversationService.StartOrGetConversationAsync();

            var model = (
                Username: username,
                Loading: false,
                Error: (string?)null,
                ConversationId: conversation.ConversationId
            );

            return View(model);
        }

        // GET: Customer/Me
        public async Task<IActionResult> Me() // Show customer profile
        {
            if (!User.Identity.IsAuthenticated)
            {
                // If the user is not authenticated, redirect to login page.
                return RedirectToAction("Login", "Auth");
            }

            var userIdClaim = _currentUser.UserId;

            if (userIdClaim == null)
            {

                ModelState.AddModelError(string.Empty, "There no connection");
                return View();
            }

            var customer = await _customerService.GetCustomerByIdAsync(userIdClaim);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer); // View for customer
        }

        public async Task<IActionResult> EditCustomer(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            var model = new CustomerUpdateDto
            {
                UserId = customer.Acc.UserId,
                Customer = new CustomerCreateDto
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    Gender = customer.Gender,
                    IdCard = customer.IdCard,
                    UnitName = customer.UnitName,
                    TaxCode = customer.TaxCode
                }
            };

            return View("Customers/Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(CustomerUpdateDto model)
        {
            if (!ModelState.IsValid)
                return View("Customers/Edit", model);

            var updated = await _customerService.UpdateCustomerAsync(model.UserId, model.Customer);
            return updated ? RedirectToAction("Customers") : NotFound();
        }
    }
}