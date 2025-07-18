using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace DiamondAssessmentSystem.Controllers
{
    [Authorize] //To ensure that people login before performing actions.
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ICurrentUserService currentUser, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _currentUser = currentUser;
            _logger = logger;
        }

        // GET: Employee/Me
        [HttpGet]
        public async Task<IActionResult> Me() // Changed from GetMyEmployee
        {
            //Authorization, to ensure access
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized attempt to access Employee profile.");
                return RedirectToAction("Login", "Auth");   //Redirect 
            }

            var userId = _currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                //Log with messages
                ModelState.AddModelError(string.Empty, "Please login before trying to see profile.");
                return View(); //Returns to page
            }

            var employee = await _employeeService.GetEmployees(userId);
            if (employee == null)
            {
                //Log with messages if unable to find it.
                ModelState.AddModelError(string.Empty, "Cannot Find Data.");
                return View(); // If not found, return the same message
            }

            //Returns to show results and view
            return View(employee);  //Load, View is required or it would say that there are no value to load.
        }

        // GET: Employee/EditMe
        [HttpGet]
        public async Task<IActionResult> Edit()  //Prepares the View before Posting
        {
            //Require login.
            if (!User.Identity.IsAuthenticated)
            {
                // Log errors
                return RedirectToAction("Login", "Auth");
            }

            //Get data to then call
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId)) return View("Unauthorized");


            var employee = await _employeeService.GetEmployees(userId);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employee/EditMe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMe(EmployeeDto employeeDto)   //Posts the value in.
        {
            //Requires login
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            var userId = _currentUser.UserId;
            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "Cannot have empty field");
                return View("Edit", employeeDto);
            }

            //Validation and test that all code passes.
            if (!ModelState.IsValid)
            {
                return View(employeeDto); //Recreate the page if there are problems.
            }

            try
            {
                //Test and run as test!
                var updated = await _employeeService.UpdateEmployee(userId, employeeDto);

                if (!updated)
                {
                    return View("NotFound");  //This may not be needed.

                }

                return RedirectToAction("Me");
            }
            catch (Exception ex)
            {
                //Log with log code
                ModelState.AddModelError(string.Empty, $"Details not validated");
                return View("Edit", employeeDto);
            }
        }

        // GET: Employee/Edit/{userId}
        //Needs more function to make to edit it
        public IActionResult Unauthorized()    //Function to have when they login
        {
            return View();
        }
    }
}