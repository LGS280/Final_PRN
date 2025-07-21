using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;  // For SelectList
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    //  [Route("[controller]")] // Removed route attribute
    public class AuthController : Controller // Inherit from Controller instead of ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login() //Method to load current login and authenticate
        {
            return View(); //Return view, after all
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto) // Pass LoginDto as a parameter
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            try
            {
                var loginResponse = await _authService.LoginAsync(loginDto);

                HttpContext.Session.SetString("access_token", loginResponse.Token);


                //After authenticating, will be to redirect page
                return RedirectToAction("Index", "Home");

            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message); //Show Error if cannot authenticate
                return View(loginDto);

            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(loginDto);
            }
        }
        // GET: Auth/Register
        [HttpGet]
        public IActionResult RegisterCustomer() //Get register page to create user accounts
        {
            return View();
        }

        // POST: Auth/RegisterCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)  // Take user from account
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = await _authService.RegisterCustomerAsync(registerDto);
                    return RedirectToAction("Login", "Auth");  // If register success, you be sent back to the login page
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(registerDto);

                }
                catch (Exception ex)
                {
                    //Log errors
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                    return View(registerDto);

                }
            }

            return View(registerDto);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
                 {
                     RedirectUri = "/Auth/Login"  // Redirect url to login after
                 });

        }

    }
}