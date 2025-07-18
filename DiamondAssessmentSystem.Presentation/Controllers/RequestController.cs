using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DiamondAssessmentSystem.Controllers
{
    [Authorize]  // All requires authentication
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ICurrentUserService _currentUser;

        public RequestController(IRequestService requestService, ICurrentUserService currentUser)
        {
            _requestService = requestService;
            _currentUser = currentUser;
        }

        // GET: Request
        public async Task<IActionResult> Index() // Changed from GetAllRequests
        {
            var requests = await _requestService.GetRequestsAsync();
            return View(requests);  // Returns the data
        }

        // GET: Request/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Request/My
        public async Task<IActionResult> My() // Changed from GetMyRequests
        {
            if (!User.Identity.IsAuthenticated) //Check to ensure user authentication
            {
                //If unauthenticated direct them to Login
                return RedirectToAction("Login", "Auth");
            }

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                // if there is a userid issue, load to new page/error for the user.
                return View("UnAuthorized");
            }

            var requests = await _requestService.GetRequestsByCustomerIdAsync(userId);

            return View(requests);  //Load and direct data into action.
        }

        // GET: Request/Create
        public IActionResult Create() //Loads the data to create a profile
        {
            return View();  // Returns to View that allows for creating of Request
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(RequestCreateDto createDto, string action)
        {
            //Validate for Authenticity
            if (!User.Identity.IsAuthenticated)
            {
                //If user cannot access to page, reload login
                return RedirectToAction("Login", "Auth");
            }
            //Validate the codes
            if (!ModelState.IsValid)
            {
                //Model is what will show, if the model is unvalided it reloads it to fill it in more.
                return View("Unauthorized");
            }
            try
            {
                //Function to validate
                var userId = _currentUser.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "User Cannot be blank in current state, check back and try again."); //Test
                    return View();  //Goes back to error
                }

                //What action is to be performed with this
                string status;
                //Creates pending or draft
                if (action == "Submit")
                {
                    status = "Pending";
                }
                else
                {
                    status = "Draft"; //Creates, by default, will send it to Draft

                }

                //Make the data and create, while loading those information to be added to the function
                var created = await _requestService.CreateRequestForCustomerAsync(userId, createDto, status);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Can’t connect to Database. Message:" + ex);
                return View(); // return it
            }
        }

        // GET: Request/Cancel/{id}
        [HttpPost, ActionName("Cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Validate for user 
            var userId = _currentUser.UserId;

            // Validate action based on request (only to see code can only be done on the right area)
            if (userId == null)
            {
                //Return Error if no User Id, and if the system isn't able to retrieve.
                return RedirectToAction("Error");
            }

            // Try to cancel,
            try
            {
                //Perform action
                var success = await _requestService.CancelRequestAsync(userId, id);
                return RedirectToAction("Index");   //Goes home 
            }
            catch (Exception ex)
            {
                //If system is not, or there is incorrect validation
                ModelState.AddModelError(string.Empty, $"Request could not be validate \n  Error is {ex}");
                return View();  //Sends back code to test, it works!
            }
        }
        // GET: Request/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var requests = await _requestService.GetRequestByIdAsync(id);

            if (requests == null)
            {
                return NotFound(); //If not found, return error
            }

            // Ngăn không cho edit nếu status là Pending
            if (requests.Status == "Pending")
            {
                TempData["ErrorMessage"] = "Cannot edit a request that is already pending.";
                return RedirectToAction("Details", new { id });
            }

            var requestCreateDto = new RequestCreateDto
            {
                ServiceId = requests.ServiceId,
                RequestType = requests.RequestType,
                // Add other properties as needed
            };

            ViewBag.Id = requests.RequestId; // Store the request ID in ViewBag for use in the view

            return View(requestCreateDto);   //Return list of what we have
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RequestCreateDto updateDto)    //POSTs and put new function.
        {
            //Requires Code to make it work
            if (!ModelState.IsValid)
                return View();   //if error please load code and see what happened.



            try
            {
                var updated = await _requestService.UpdateRequestAsync(id, updateDto);

                if (!updated)
                {
                    //If error redirect
                    return View("Unauthorized");
                }

                //If that is not the situation, return the proper value
                return RedirectToAction(nameof(Index)); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please validate code.");
                return View(ex.Message); // returns original
            }
        }
    }
}