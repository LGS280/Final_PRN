using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Enums;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly IAccountService _accountService;

        public AdminController(
            ICustomerService customerService,
            IEmployeeService employeeService,
            IAccountService accountService)
        {
            _customerService = customerService;
            _employeeService = employeeService;
            _accountService = accountService;
        }

        // ===== CUSTOMER MANAGEMENT =====
        public async Task<IActionResult> Customers()
        {
            var data = await _customerService.GetAllCustomersAsync();
            return View("Customers/Index", data);
        }

        public async Task<IActionResult> EditCustomer(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            var dto = new CustomerUpdateDtoV2
            {
                UserId = id,
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

            return View("Customers/Edit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(CustomerUpdateDtoV2 model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PopupType = "error";
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.PopupMessage = "Invalid input:<br/>" + string.Join("<br/>", errors);
                return View("Customers/Edit", model);
            }

            var updated = await _customerService.UpdateCustomerAsync(model);
            ViewBag.PopupType = updated ? "success" : "error";
            ViewBag.PopupMessage = updated ? "Customer updated successfully." : "Failed to update customer.";

            return View("Customers/Edit", model);
        }

        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Customers));
        }

        public async Task<IActionResult> CustomerDetails(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return View("Customers/Details", customer);
        }

        public async Task<IActionResult> DeactivateCustomer(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            var result = await _accountService.UpdateStatusAsync(id, "Inactive");
            return RedirectToAction(nameof(Customers));
        }

        public async Task<IActionResult> ActivateCustomer(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            var result = await _accountService.UpdateStatusAsync(id, "Active");
            return RedirectToAction(nameof(Customers));
        }

        // ===== EMPLOYEE MANAGEMENT =====
        public async Task<IActionResult> Employees()
        {
            var data = await _employeeService.GetAllEmployeesAsync();
            return View("Employees/Index", data);
        }

        public async Task<IActionResult> EditEmployee(string id)
        {
            var emp = await _employeeService.GetEmployees(id);
            if (emp == null) return NotFound();

            var dto = new EmployeeUpdateDto
            {
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Phone = emp.Phone,
                Gender = emp.Gender,
                Salary = emp.Salary
            };

            ViewBag.UserId = emp.UserId;
            ViewBag.Email = emp.Email;

            return View("Employees/Edit", dto);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmployee(string id, EmployeeUpdateDto dto)
        {
            ViewBag.UserId = id;

            if (!ModelState.IsValid)
            {
                var email = await _employeeService.GetEmployeeEmail(id);
                ViewBag.Email = email;
                ViewBag.PopupType = "error";
                ViewBag.PopupMessage = "Invalid input. Please check and try again.";
                return View("Employees/Edit", dto);
            }

            var result = await _employeeService.UpdateEmployee(id, dto);
            var emailAfterUpdate = await _employeeService.GetEmployeeEmail(id);
            ViewBag.Email = emailAfterUpdate;

            switch (result)
            {
                case EmployeeEnum.Success:
                    ViewBag.PopupType = "success";
                    ViewBag.PopupMessage = "Employee updated successfully.";
                    break;

                case EmployeeEnum.InvalidPhoneNumber:
                    ViewBag.PopupType = "error";
                    ViewBag.PopupMessage = "Invalid phone number.";
                    ModelState.AddModelError(nameof(dto.Phone), "Invalid phone number.");
                    break;

                case EmployeeEnum.NotFound:
                    ViewBag.PopupType = "error";
                    ViewBag.PopupMessage = "Employee not found.";
                    ModelState.AddModelError(string.Empty, "Employee not found.");
                    break;

                case EmployeeEnum.UpdateFailed:
                    ViewBag.PopupType = "error";
                    ViewBag.PopupMessage = "Failed to update employee.";
                    ModelState.AddModelError(string.Empty, "Update failed. Please try again.");
                    break;

                default:
                    ViewBag.PopupType = "error";
                    ViewBag.PopupMessage = "An unknown error occurred.";
                    ModelState.AddModelError(string.Empty, "An unknown error occurred.");
                    break;
            }

            return View("Employees/Edit", dto);
        }

        public async Task<IActionResult> DeleteEmployee(string id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return RedirectToAction(nameof(Employees));
        }

        public async Task<IActionResult> EmployeeDetails(string id)
        {
            var emp = await _employeeService.GetEmployees(id);
            if (emp == null) return NotFound();
            return View("Employees/Details", emp);
        }

        public async Task<IActionResult> DeactivateEmployee(string id)
        {
            var result = await _accountService.UpdateStatusAsync(id, "Inactive");
            return RedirectToAction(nameof(Employees));
        }

        public async Task<IActionResult> ActivateEmployee(string id)
        {
            var result = await _accountService.UpdateStatusAsync(id, "Active");
            return RedirectToAction(nameof(Employees));
        }

        // ===== ACCOUNT CREATION FOR EMPLOYEE =====
        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View("Accounts/Create");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(RegisterEmployeesDto dto)
        {
            if (!ModelState.IsValid)
                return View("Accounts/Create", dto);

            try
            {
                var created = await _accountService.CreateEmployeeAsync(dto, dto.Role);
                TempData["Success"] = "Employee account created successfully.";
                return RedirectToAction(nameof(Employees)); 
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View("Accounts/Create", dto);
            }
        }

    }
}
