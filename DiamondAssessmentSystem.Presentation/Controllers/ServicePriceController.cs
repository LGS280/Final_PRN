using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class ServicePriceController : Controller
    {
        private readonly IServicePriceService _servicePriceService;

        public ServicePriceController(IServicePriceService servicePriceService)
        {
            _servicePriceService = servicePriceService;
        }

        // GET: ServicePrice
        public async Task<IActionResult> Index()
        {
            var result = await _servicePriceService.GetAllAsync();
            if (result == null || !result.Any())
            {
                return View("Empty"); // Handle empty List.
            }
            return View(result);
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
        public IActionResult Create() //If the check passes then return to what there will be
        {
            //Returns what is valid. 
            return View();
        }

        // POST: ServicePrice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServicePriceCreateDto dto)
        {

            //For this one we have to check and then add
            if (ModelState.IsValid)
            {

                try
                {
                    var created = await _servicePriceService.CreateAsync(dto);
                    return RedirectToAction(nameof(Index));   //Once it works, return
                }
                //If it's something that needs to be verified please report.
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Request could not be sent \n More details: {ex}");
                    //Just to make sure, code is running.
                    return View();
                }
            }
            return View(); //If all those code are not valid, code reloads to the page
        }

        // GET: ServicePrice/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)   //Getting what will be editted.
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
                Status = ServicePrice.Status,
                EmployeeId = ServicePrice.EmployeeId
            };

            ViewBag.Id = ServicePrice.ServiceId; // Store the service ID in ViewBag for use in the view

            //Loads code, will be edited.
            return View(servicePriceDto);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServicePriceCreateDto dto)
        {
            //Make sure is correct.
            if (!ModelState.IsValid)
                return View();

            try
            {
                //Validate before calling what will be
                var updated = await _servicePriceService.UpdateAsync(id, dto);
                //Return what the user did, or it may lead to what user does.
                if (!updated)
                {
                    ModelState.AddModelError(string.Empty, "Lỗi update");
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