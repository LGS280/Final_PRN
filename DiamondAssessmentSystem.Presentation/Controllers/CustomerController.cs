using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization; // Required
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class CustomerController : Controller 
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
        public async Task<IActionResult> Me() 
        {
            if (!User.Identity.IsAuthenticated)
            {
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

            return View(customer); 
        }

        public async Task<IActionResult> EditCustomer()
        {
            var userIdClaim = _currentUser.UserId;

            if (userIdClaim == null)
            {

                ModelState.AddModelError(string.Empty, "There no connection");
                return View();
            }

            var customer = await _customerService.GetCustomerByIdAsync(userIdClaim);
            if (customer == null) return NotFound();

            var model = new CustomerUpdateDto
            {
                UserId = customer.Acc.UserId,
                Customer = new CustomerCreateDto
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Gender = customer.Gender,
                    Phone = customer.Phone,
                    IdCard = customer.IdCard,
                    Address = customer.Address,
                    UnitName = customer.UnitName,
                    TaxCode = customer.TaxCode,
                    Email = customer.Email
                }
            };

            return View("EditCustomer", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(CustomerUpdateDto model)
        {
            if (!ModelState.IsValid)
                return View("EditCustomer", model);

            var updated = await _customerService.UpdateCustomerAsync(model.UserId, model.Customer);
            return updated ? RedirectToAction("Customers") : NotFound();
        }

        public async Task<IActionResult> EditCustomerVer1()
        {
            var userIdClaim = _currentUser.UserId;

            if (userIdClaim == null)
            {
                ModelState.AddModelError(string.Empty, "There is no connection");
                return View("EditCustomerVer1", new CustomerUpdateDtoVer1());
            }

            var customer = await _customerService.GetCustomerByIdAsync(userIdClaim);
            if (customer == null) return NotFound();

            var model = new CustomerUpdateDtoVer1
            {
                UserId = customer.Acc.UserId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Gender = customer.Gender,
                Phone = customer.Phone,
                IdCard = customer.IdCard,
                Address = customer.Address,
                UnitName = customer.UnitName,
                TaxCode = customer.TaxCode,
                Email = customer.Email,
                UserName = customer.UserName
            };

            return View("EditCustomerVer1", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomerVer1(CustomerUpdateDtoVer1 model)
        {
            if (!ModelState.IsValid)
                return View("EditCustomerVer1", model);

            var customerDto = new CustomerCreateDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Phone = model.Phone,
                IdCard = model.IdCard,
                Address = model.Address,
                UnitName = model.UnitName,
                TaxCode = model.TaxCode,
                Email = model.Email,
                UserName = model.UserName
            };

            try
            {
                var updated = await _customerService.UpdateCustomerAsync(model.UserId, customerDto);
                if (!updated)
                {
                    TempData["Error"] = "Update failed. Please try again.";
                    return View("EditCustomerVer1", model);
                }

                TempData["UpdateSuccess"] = "Profile updated successfully!";
                return RedirectToAction("EditCustomerVer1");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View("EditCustomerVer1", model);
            }
        }


    }
}