using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DiamondAssessmentSystem.Controllers
{

    public class CertificateController : Controller
    {
        private readonly ICerterficateService _certificateService;
        private readonly ICurrentUserService _currentUser;

        public CertificateController(ICerterficateService certificateService, ICurrentUserService currentUser)
        {
            _certificateService = certificateService;
            _currentUser = currentUser;
        }

        // GET: Certificate/Management
        public async Task<IActionResult> Management() // Changed from GetCertificates
        {
            //You must be authorized to acess this.
            var certificates = await _certificateService.GetCertificatesAsync();
            return View(certificates);  // Returns results.
        }

        // GET: Certificate
        public async Task<IActionResult> Personal() // Changed from GetPersonalCertificates
        {
            //Require user to check it exists otherwise
            if (!User.Identity.IsAuthenticated)
            {
                //Return to login if they don't have a valid login
                return RedirectToAction("Login", "Auth");
            }

            var userId = _currentUser.UserId;

            if (userId == null)
            {
                return View("Unauthorized");
            }
            var certificates = await _certificateService.GetPersonalCertificates(userId);

            return View(certificates);
        }

        // GET: Certificate/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var certificate = await _certificateService.GetCertificateByIdAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate); // Return the view if the certificate is valid
        }

        // GET: Certificate/Create
        [HttpGet]
        public IActionResult Create() //Loads the page
        {
            return View();  // Returns to View that allows for creating of a certificate
        }

        // POST: Certificate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]  //For Protection
        public async Task<IActionResult> Create(CertificateCreateDto certificateCreateDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Log activity that will cause a change to databases to prevent errors
                    var createdCertificate = await _certificateService.CreateCertificateAsync(certificateCreateDto);
                    return RedirectToAction(nameof(Management));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error with information occured.");
                    return View(); //Return error message
                }

            }

            return View();   //Validate to the best it can
        }

        // GET: Certificate/Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)  //Find Id to be editted, returns to Edit if exist
        {
            var certificate = await _certificateService.GetCertificateByIdAsync(id);
            if (certificate == null) return NotFound(); // if not existing, return error

            return View(certificate); //If there does it returns.
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(CertificateCreateDto certificateCreateDto)
        { //Check to the best it is able to validate and Edit
          //Validate Code here.

            if (!User.Identity.IsAuthenticated)
            {
                //Return to login if they are not.
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                var userId = _currentUser.UserId;

                //Validation of new update
                if (userId == null)
                {
                    ModelState.AddModelError(string.Empty, "User is empty");
                    return View();
                }


                //Validate the codes and functions
                var updated = await _certificateService.UpdateCertificateAsync(userId, certificateCreateDto);

                if (!updated)
                {
                    return NotFound(); //If unable to work
                }

                return RedirectToAction(nameof(Management)); //returns for next function to process!
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "There are no function, will do nothing");
                return View();   //will only return to the page as no change.
            }
            return View();   //Returns as this check is last
        }
    }
}