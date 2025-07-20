using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace DiamondAssessmentSystem.Presentation.Controllers
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
        public async Task<IActionResult> Edit()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId)) return View("Unauthorized");

            var employee = await _employeeService.GetEmployees(userId);
            if (employee == null) return NotFound();

            var dto = new EmployeeUpdateDto
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Phone = employee.Phone,
                Gender = employee.Gender,
                Salary = employee.Salary
            };

            return View(dto);
        }

        // POST: Employee/EditMe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMe(EmployeeUpdateDto dto)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Invalid user.");
                return View("Edit", dto);
            }

            if (!ModelState.IsValid)
                return View("Edit", dto);

            try
            {
                var updated = await _employeeService.UpdateEmployee(userId, dto);
                return updated ? RedirectToAction("Me") : NotFound();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating.");
                return View("Edit", dto);
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