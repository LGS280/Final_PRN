using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;  // For SelectList

namespace DiamondAssessmentSystem.Controllers
{
    [Authorize(Roles = "Consultant")] //Requires log in
    public class ResultController : Controller
    {
        private readonly IResultService _resultService;
        private readonly ICurrentUserService _currentUser;

        public ResultController(IResultService resultService, ICurrentUserService currentUser)
        {
            _resultService = resultService;
            _currentUser = currentUser;
        }
        // GET: Result

        public async Task<IActionResult> Index() // GET: Results
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");   //Redirect 
            }
            var results = await _resultService.GetResultsAsync();
            return View(results);  //Return as the List of the Code
        }

        // GET: Result/by-customer
        public async Task<IActionResult> ByCustomer(int customerId)  //To be used if user wants to find based on ID, will not use.
        {
            var results = await _resultService.GetResultsAsync(customerId);
            return View(results);
        }

        //GET results based on the current user ID.   
        // GET: Result/My
        public async Task<IActionResult> MyResult() //To check to have ID, to create to be more unique
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");   //Redirect 
            }

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                //Load message for the user to load proper page.
                return View("Unauthorized");//returns default, and sends to Error Page
            }

            var results = await _resultService.GetPersonalResults(userId);
            return View(results);  //Return to the view, where all the properties are!
        }

        //GET Result details
        // GET: Result/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _resultService.GetResultByIdAsync(id);
            //If nothing is loaded, then the code runs.
            if (result == null)
            {
                return NotFound();  //Returns the Error if things cannot be load as there are not function for it to happen.
            }

            return View(result); //Load and return view, as properties are found
        }
        //GET Create Action.
        // GET: Result/Create
        public IActionResult Create()
        {
            //Load for Creation Action to work
            return View();  // Loads view that will allow for users to be added
        }

        //Function to be loaded at the end.   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResult(int orderId, ResultCreateDto createDto)    //Posts the value in create DTO
        {
            //Validate to ensure they are real, so the function don't bypass
            if (!User.Identity.IsAuthenticated)
            {
                //This to check when user not active return there
                return RedirectToAction("Login", "Auth");
            }
            //Make to make to not have exception, since the view can be bypassed.
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _currentUser.UserId;
                    bool createdResult = await _resultService.CreateResultAsync(orderId, createDto);

                    if (createdResult == false)
                    {
                        return View("Unauthorized"); //Return default is has not function to proceed
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    //Exception if all is wrong, is at the door way, so you don't get error report.
                    ModelState.AddModelError(string.Empty, $"Please validate the code or report");
                    //Load original, and that nothing exists anymore.
                    return View();
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _resultService.GetResultByIdAsync(id);
            //Load
            if (result == null)
            {
                return NotFound();
            }

            var res = new ResultCreateDto
            {
                DiamondId = result.DiamondId,
                RequestId = result.RequestId,
                DiamondOrigin = result.DiamondOrigin,
                Shape = result.Shape,
                Measurements = result.Measurements,
                CaratWeight = result.CaratWeight,
                Color = result.Color,
                Clarity = result.Clarity,
                Cut = result.Cut,
                Proportions = result.Proportions,
                Polish = result.Polish,
                Symmetry = result.Symmetry,
                Fluorescence = result.Fluorescence,
                Status = result.Status
            };

            return View(res);  //Validate, and send
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ResultCreateDto resultCreateDto)    //POST is not working, will use default.
        {
            if (resultCreateDto == null)
            {
                return NotFound();
            }
            //Validate to ensure they are real, so the function don't bypass
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            //This if validation does not pass.
            if (!ModelState.IsValid)
            {
                return View(); //Goes back to make it correct or tell them the error
            }

            try
            {
                var updated = await _resultService.UpdateResultAsync(id, resultCreateDto);
                //Cannot perform
                if (!updated)
                {
                    return View("Unauthorized");
                }

                return RedirectToAction(nameof(Index));  //If it works will send to the function to list to work
            }
            catch (Exception ex)
            {
                //Validate before working and to be more safe.
                ModelState.AddModelError(string.Empty, "Please enter all values in the page, and return a value");
                return View();   //Sends back validation
            }
        }

        // GET: Result/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //get data to show, else there may be nothing there
            var result = await _resultService.GetResultByIdAsync(id);
            //If it is bad information, then report
            if (result == null)
            {
                //Return to a new request as the project did not have it.
                return NotFound();
            }

            return View(result);   //Validate is all is correct.
        }

        // POST: Result/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteResult(int id)
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }
            var action = await _resultService.DeleteResultAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}