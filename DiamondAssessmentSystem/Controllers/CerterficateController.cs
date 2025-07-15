using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly ICerterficateService _certificateService;
        private readonly ICurrentUserService _currentUser;

        public CertificateController(ICerterficateService certificateService, ICurrentUserService currentUser)
        {
            _certificateService = certificateService;
            _currentUser = currentUser;
        }

        //[Authorize(Roles = "Manager")]
        [HttpGet("Management")]
        public async Task<ActionResult<IEnumerable<CertificateDto>>> GetCertificates()
        {
            var certificates = await _certificateService.GetCertificatesAsync();
            return Ok(certificates);
        }

        //[Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CertificateDto>>> GetPersonalCertificates()
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                return Unauthorized();

            var certificates = await _certificateService.GetPersonalCertificates(userId);
            return Ok(certificates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CertificateDto>> GetCertificate(int id)
        {
            var certificate = await _certificateService.GetCertificateByIdAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            return Ok(certificate);
        }

        // POST: api/Certificate
        [HttpPost]
        public async Task<ActionResult<CertificateDto>> PostCertificate(CertificateCreateDto certificateCreateDto)
        {
            var createdCertificate = await _certificateService.CreateCertificateAsync(certificateCreateDto);
            return CreatedAtAction(nameof(GetCertificate), new { id = createdCertificate.CertificateId}, createdCertificate);
        }

        //[Authorize(Roles = "Manager")]
        [HttpPut]
        public async Task<IActionResult> PutCertificate(CertificateCreateDto certificateCreateDto)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                return Unauthorized();

            var updated = await _certificateService.UpdateCertificateAsync(userId, certificateCreateDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
