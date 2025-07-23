using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ServicePriceController(IServicePriceService servicePriceService, ICurrentUserService currentUser, IEmployeeService employeeService)
        {
            _servicePriceService = servicePriceService;
            _currentUser = currentUser;
            _employeeService = employeeService;
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
                //Handle, when there isn't a lot to process
                return View("Empty");
            }

            return View(result);
        }
        public async Task<IActionResult> Details(int id)
        {
            var ServicePrice = await _servicePriceService.GetByIdAsync(id);

            if (ServicePrice == null)
            {
                //If it never loads what we need, then we need to return an error.
                return View("NotFound");
            }

            return View(ServicePrice);  //If there, we can finally load all of it
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
                    return RedirectToAction(nameof(Index));   //Once it works, return
                }
                //If it's something that needs to be verified please report.
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
            //Find if there or not
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

            //Make sure is correct.
            if (!ModelState.IsValid)
                return View();

            try
            {
                //Validate before calling what will be
                var updated = await _servicePriceService.UpdateAsync(id, dto, userId);
                //Return what the user did, or it may lead to what user does.
                if (!updated)
                {
                    ModelState.AddModelError(string.Empty, "Error update");
                    return View(dto);  //If it does not work, return to the same page.
                }

                return RedirectToAction(nameof(Index));   //Return function
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please have all code or values in place.");
                //Returns and reload the error.
                return View(ex.Message);
            }
        }

        // GET: ServicePrice/Delete/{id}
        public async Task<IActionResult> Delete(int id)  //Gets the Method and Loads to Confirm
        {
            //Get data to show, else there may be nothing there
            var result = await _servicePriceService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();   //Returns page to reload
            }

            //Validate that there is correct code and working function.
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

            // Try to cancel, Validate that you see.
            try
            {
                //Delete action to make sure is deleted and it's function
                var action = await _servicePriceService.SoftDeleteAsync(id);
                //And load and work
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Unable to validate system \n Returning action code");
                return View(); //Trys to load code back if unable to remove!
            }
        }
    }
}