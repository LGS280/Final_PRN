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
    public class AuthController : Controller 
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login() 
        {
            return View(); 
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto) 
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            try
            {
                var loginResponse = await _authService.LoginAsync(loginDto);

                HttpContext.Session.SetString("access_token", loginResponse.Token);

                return RedirectToAction("Index", "Home");

            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginDto);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(loginDto);
            }
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult RegisterCustomer() 
        {
            return View();
        }

        // POST: Auth/RegisterCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = await _authService.RegisterCustomerAsync(registerDto);
                    TempData["Success"] = "Registration successful. Please log in.";
                    return RedirectToAction("Login", "Auth");
                }
                catch (ArgumentException ex)
                {
                    TempData["Error"] = ex.Message;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return View(registerDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");

        }

    }
}