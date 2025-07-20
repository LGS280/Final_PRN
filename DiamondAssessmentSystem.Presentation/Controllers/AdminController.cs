using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
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

            var updateDto = new CustomerUpdateDto
            {
                UserId = customer.Acc?.UserId,
                Customer = new CustomerCreateDto
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    IdCard = customer.IdCard,
                    Gender = customer.Gender,
                    UnitName = customer.UnitName,
                    TaxCode = customer.TaxCode,
                }
            };

            return View("Customers/Edit", updateDto); 
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(CustomerUpdateDto model)
        {
            if (!ModelState.IsValid)
                return View("Customers/Edit", model);

            var updated = await _customerService.UpdateCustomerAsync(model.UserId, model.Customer);
            return updated ? RedirectToAction(nameof(Customers)) : NotFound();
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
            if (!ModelState.IsValid)
            {
                ViewBag.UserId = id;
                return View("Employees/Edit", dto);
            }

            var updated = await _employeeService.UpdateEmployee(id, dto);
            return updated ? RedirectToAction(nameof(Employees)) : NotFound();
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
                return RedirectToAction(nameof(Employees));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Accounts/Create", dto);
            }
        }
    }
}
