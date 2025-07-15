using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Required
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    //[Authorize]   Requires authentication
    public class CustomerController : Controller // MVC controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICurrentUserService _currentUser;

        public CustomerController(ICustomerService customerService, ICurrentUserService currentUser)
        {
            _customerService = customerService;
            _currentUser = currentUser;
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

        // GET: Customer/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()  //Load Edit Account
        {
            var userIdClaim = _currentUser.UserId;

            if (!User.Identity.IsAuthenticated)
            {
                //Return to login if they are not.
                return RedirectToAction("Login", "Auth");
            }

            if (userIdClaim == null)
            {
                return View("UnAuthorized");  //Loads a new page.
            }

            var customer = await _customerService.GetCustomerByIdAsync(userIdClaim);

            return View(customer); //Load edit user info
        }

        // POST: Customer/EditPost
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerCreateDto customerCreateDto)
        {
            //This method checks for authorization and for correct Id to transfer over
            var userId = _currentUser.UserId;
            if (userId == null)
            {
                return View("UnAuthorized"); //Load user, if not found
            }

            //check if they can log in again.
            if (ModelState.IsValid)
            {
                try
                {
                    //Attempt
                    var updated = await _customerService.UpdateCustomerAsync(userId, customerCreateDto);
                    if (!updated)
                    {
                        //return if there is no action possible.
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Me)); //return if successful
                }

                //Error message that shows if an exception occurs.
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error with code {ex}");
                    return View(customerCreateDto);
                }
            }
            return View(); //If the id wasn't there and can load the current data
        }
    }
}