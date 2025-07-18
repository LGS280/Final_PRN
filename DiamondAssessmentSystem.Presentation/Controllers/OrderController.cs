using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DiamondAssessmentSystem.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUser;

        public OrderController(IOrderService orderService, ICurrentUserService currentUser)
        {
            _orderService = orderService;
            _currentUser = currentUser;
        }

        // GET: Order
        public async Task<IActionResult> MyOrder()   //Get all order
        {

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                // If not authenticated, redirect to login page.
                return RedirectToAction("Login", "Auth");
            }

            var orders = await _orderService.GetOrdersByCustomerAsync(userId); //loads with user
            return View(orders);
        }

        // GET: Order/All
        //[Authorize(Roles = "Admin")]//To require authentication here. - for extra code when able too-
        public async Task<IActionResult> Index()  //Get all types to control order (To edit code and more).
        {
            var orders = await _orderService.GetOrdersAsync();
            return View(orders);
        }

        // GET: Order/Details/{id}
        public async Task<IActionResult> Details(int id) //View detail of each line.
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();  //Test each and make to test well so it looks good
            }
            return View(order);  //Load

        }

        // GET: Order/Create
        public IActionResult Create()  // To have code for a profile (page)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentInfo, OrderData")] OrderCreateCombineDto order)    //Take form that will be send over with all parts of the code
        {
            //Validate the Code for Login - IMPORTANT
            var userId = _currentUser.UserId;
            //Authenticate
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Auth");

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Authentication not found. Make sure that is not invalid");
                return View();
            }
            // Validate that has every code they need
            if (ModelState.IsValid)
            {
                try
                {
                    //Check to see if it connects

                    var created = await _orderService.CreateOrderAsync(
                       userId,
                       order.PaymentInfo.RequestId,
                       order.OrderData,
                       order.PaymentInfo.PaymentType,
                       order.PaymentInfo.PaymentRequest);

                    if (!created)
                    {
                        //If there is, what action to prevent it and load
                        ModelState.AddModelError(string.Empty, "Request for a new page did not fully load");
                        return View(CreateView());
                    }

                    return RedirectToAction(nameof(Index)); //Return here
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"If any errors, what do you want the pages to do. \nMessage{ex.Message}");
                    return View(ex.Message); //Return 
                }
            }
            return View(order); //Test the loading, will happen if there is bad request.
        }

        // GET: Order/Edit/{id}
        public IActionResult Edit(int Id)
        {
            return View();   // To do in the future.
        }

        // GET: Order/Cancel/{id}
        [HttpPost, ActionName("Cancel")]
        public async Task<IActionResult> CancelOrder(int id)   //Posts to canel a confirm file.
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                //Get the user Id.
                var userId = _currentUser.UserId;
                var success = await _orderService.CancelOrderAsync(id);

                if (!success)
                {
                    //If there is still errors to complete.
                    ModelState.AddModelError(string.Empty, "An update with current process will not let this request validate, try to resolve this.");
                    return View();  //What happens with cancel, goes back to start
                }

                return RedirectToAction(nameof(Index));   //Load the Home Page
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Cannot validate any action. \n Error{ex}");
                return RedirectToAction(nameof(Index));   //What is it will occur that will be the outcome.
            }
        }


        public ActionResult CreateView()  //Loads Create
        {
            return View();   //Return to code to test and make sure it loads.
        }


        // PUT: api/Order/payment
        public ActionResult payment()   //Payments for code, not sure to check payment codes from API in Controller.
        {
            return View();
        }
    }
}