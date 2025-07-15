using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicePriceController : ControllerBase
    {
        private readonly IServicePriceService _servicePriceService;

        public ServicePriceController(IServicePriceService servicePriceService)
        {
            _servicePriceService = servicePriceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicePriceDto>>> GetAll()
        {
            var result = await _servicePriceService.GetAllAsync();

            if (result == null || !result.Any())
                return NoContent();

            return Ok(result);
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<ServicePriceDto>>> GetByStatus(string status)
        {
            var result = await _servicePriceService.GetByStatusAsync(status);

            if (result == null || !result.Any())
                return NoContent();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServicePriceDto>> GetById(int id)
        {
            var servicePrice = await _servicePriceService.GetByIdAsync(id);

            if (servicePrice == null)
                return NotFound($"Service price with ID {id} not found.");

            return Ok(servicePrice);
        }

        [HttpPost]
        public async Task<ActionResult<ServicePriceDto>> Create([FromBody] ServicePriceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _servicePriceService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.ServiceId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServicePriceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _servicePriceService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound($"Service price with ID {id} not found.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var deleted = await _servicePriceService.SoftDeleteAsync(id);

            if (!deleted)
                return NotFound($"Service price with ID {id} not found.");

            return NoContent();
        }
    }
}
