using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.Models;
using DiamondAssessmentSystem.Presentation.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class ServicePriceController : Controller
    {
        private readonly IServicePriceService _servicePriceService;
        private readonly ICurrentUserService _currentUser;
        private readonly IEmployeeService _employeeService;
        private readonly IHubContext<ServicePriceHub> _hubContext;
        public ServicePriceController(IServicePriceService servicePriceService, 
            ICurrentUserService currentUser, IEmployeeService employeeService,
            IHubContext<ServicePriceHub> hubContext)
        {
            _servicePriceService = servicePriceService;
            _currentUser = currentUser;
            _employeeService = employeeService;
            _hubContext = hubContext;
        }

        // GET: ServicePrice
        public async Task<IActionResult> Index()
        {
            var services = await _servicePriceService.GetAllAsync() ?? new List<ServicePriceDto>();

            foreach (var service in services)
            {
                var user = await _employeeService.GetUserById(service.EmployeeId); 

                service.EmployeeName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown";
            }

            return View(services);
        }

        public async Task<IActionResult> Status(string status)
        {
            var result = await _servicePriceService.GetByStatusAsync(status);

            if (result == null || !result.Any())
            {
                return View("Empty");
            }

            return View(result);
        }
        public async Task<IActionResult> Details(int id)
        {
            var ServicePrice = await _servicePriceService.GetByIdAsync(id);

            if (ServicePrice == null)
            {
                return View("NotFound");
            }

            return View(ServicePrice);  
        }

        // GET: ServicePrice/Create
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        // POST: ServicePrice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServicePriceCreateDto dto)
        {
            var userId = _currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Please login before trying to see profile.");
                return View(); 
            }

            if (ModelState.IsValid)
            {

                try
                {
                    var created = await _servicePriceService.CreateAsync(dto, userId);
                    await _hubContext.Clients.All.SendAsync("ServicePriceChanged", "create", created);
                    return RedirectToAction(nameof(Index));   
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Request could not be sent \n More details: {ex}");

                    return View();
                }
            }
            return View(); 
        }

        // GET: ServicePrice/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id) 
        {
            var ServicePrice = await _servicePriceService.GetByIdAsync(id);
            if (ServicePrice == null) return NotFound();

            var servicePriceDto = new ServicePriceCreateDto
            {
                ServiceType = ServicePrice.ServiceType,
                Price = ServicePrice.Price,
                Description = ServicePrice.Description,
                Duration = ServicePrice.Duration,
                Status = ServicePrice.Status
            };

            ViewBag.Id = ServicePrice.ServiceId; 

            return View(servicePriceDto);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServicePriceCreateDto dto)
        {
            var userId = _currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Please login before trying to see profile.");
                return View(); 
            }

            if (!ModelState.IsValid)
                return View();

            try
            {
                var updated = await _servicePriceService.UpdateAsync(id, dto, userId);
                if (!updated)
                {
                    ModelState.AddModelError(string.Empty, "Error update");
                    return View(dto);  
                }

                await _hubContext.Clients.All.SendAsync("ServicePriceChanged", "edit", new
                {
                    id,
                    dto.ServiceType,
                    dto.Price,
                    dto.Description,
                    dto.Duration,
                    dto.Status
                });

                return RedirectToAction(nameof(Index));   
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please have all code or values in place.");
                return View(ex.Message);
            }
        }

        // GET: ServicePrice/Delete/{id}
        public async Task<IActionResult> Delete(int id)  
        {
            var result = await _servicePriceService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();  
            }

            return View(result);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServicePrice(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var action = await _servicePriceService.SoftDeleteAsync(id);

                await _hubContext.Clients.All.SendAsync("ServicePriceChanged", "delete", new { id });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to validate system \n Returning action code");
                return View(); 
            }
        }
    }
}