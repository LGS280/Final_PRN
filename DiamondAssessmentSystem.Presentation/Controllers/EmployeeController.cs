using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Enums;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize] //To ensure that people login before performing actions.
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IReportService _reportService;

        public EmployeeController(IEmployeeService employeeService, ICurrentUserService currentUser, ILogger<EmployeeController> logger, IReportService reportService)
        {
            _employeeService = employeeService;
            _currentUser = currentUser;
            _logger = logger;
            _reportService = reportService;
        }

        // GET: Employee/Me
        [HttpGet]
        public async Task<IActionResult> Me() 
        {
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
                ModelState.AddModelError(string.Empty, "Cannot Find Data.");
                return View(); 
            }

            return View(employee);
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
                //UserName = employee.UserName
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
                var result = await _employeeService.UpdateEmployee(userId, dto);

                switch (result)
                {
                    case EmployeeEnum.Success:
                        TempData["SuccessMessage"] = "Profile updated successfully!";
                        return RedirectToAction("Me");

                    case EmployeeEnum.InvalidPhoneNumber:
                        ModelState.AddModelError(nameof(dto.Phone), "Invalid phone number.");
                        break;

                    case EmployeeEnum.NotFound:
                        ModelState.AddModelError(string.Empty, "User not found.");
                        break;

                    case EmployeeEnum.UpdateFailed:
                        ModelState.AddModelError(string.Empty, "Failed to update profile.");
                        break;

                    default:
                        ModelState.AddModelError(string.Empty, "An unknown error occurred.");
                        break;
                }

                return View("Edit", dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Edit", dto);
            }
        }


        public IActionResult Unauthorized()   
        {
            return View();
        }

        public async Task<IActionResult> ManagerDashboard(DateTime? fromDate, DateTime? toDate)
        {
            fromDate ??= DateTime.Today.AddDays(-7);
            toDate ??= DateTime.Today;

            var accountsPerDay = await _reportService.GetAccountCreatedPerDayAsync(fromDate.Value, toDate.Value);
            var totalOrders = await _reportService.GetTotalOrderCountAsync();
            var ordersByType = await _reportService.GetOrderCountByTypeAsync();
            var totalRequests = await _reportService.GetTotalRequestChosenAsync(); // thay thế pie chart
            var ordersByStatus = await _reportService.GetOrderStatusReportAsync(fromDate.Value, toDate.Value);

            var model = new ManagerDashboardDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                AccountsCreatedPerDay = accountsPerDay,
                TotalOrders = totalOrders,
                OrdersByType = ordersByType,
                TotalRequestChosen = totalRequests,
                OrderStatusStats = ordersByStatus,
            };

            return View(model);
        }
    }
}