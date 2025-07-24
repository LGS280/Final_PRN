using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class CertificateController : Controller
    {
        private readonly ICerterficateService _certificateService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public CertificateController(ICerterficateService certificateService, ICurrentUserService currentUser, IMapper mapper)
        {
            _certificateService = certificateService;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<IActionResult> Management()
        {
            var certificates = await _certificateService.GetCertificatesAsync();
            return View(certificates);
        }

        public async Task<IActionResult> Personal()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var certs = await _certificateService.GetPersonalCertificates(userId);
            return View(certs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cert = await _certificateService.GetCertificateByIdAsync(id);
            return cert == null ? NotFound() : View(cert);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificateCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _certificateService.CreateCertificateAsync(dto);
            return RedirectToAction(nameof(Management));
        }

        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var cert = await _certificateService.GetCertificateByIdAsync(id);
        //    return cert == null ? NotFound() : View(cert);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(CertificateCreateDto dto)
        //{
        //    if (!ModelState.IsValid) return View(dto);

        //    var userId = _currentUser.UserId;
        //    var updated = await _certificateService.UpdateCertificateAsync(userId, dto);
        //    if (!updated) return NotFound();

        //    return RedirectToAction(nameof(Management));
        //}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _certificateService.GetCertificateByIdAsync(id);
            if (dto == null) return NotFound();

            var editDto = _mapper.Map<CertificateEditDto>(dto);
            return View(editDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CertificateEditDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var updated = await _certificateService.UpdateCertificateAsync(dto);
            if (!updated)
            {
                ModelState.AddModelError("", "Failed to update certificate.");
                return View(dto);
            }

            return RedirectToAction("Management");
        }
    }
}