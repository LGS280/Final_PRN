using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;
        private readonly ICurrentUserService _currentUser;

        public ResultController(IResultService resultService, ICurrentUserService currentUser)
        {
            _resultService = resultService;
            _currentUser = currentUser;
        }
        
        //[Authorize(Roles = "Consultant")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetResults()
        {
            var results = await _resultService.GetResultsAsync();
            return Ok(results);
        }

        //[Authorize(Roles = "Consultant")]
        [HttpGet("by-customer")]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetResults(int customerId)
        {
            var results = await _resultService.GetResultsAsync(customerId);
            return Ok(results);
        }

        [HttpGet("cust")]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetPersonalResults()
        {

            var userId = _currentUser.UserId;

            if (userId == null)
                return Unauthorized();

            var results = await _resultService.GetPersonalResults(userId);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultDto>> GetResult(int id)
        {
            var result = await _resultService.GetResultByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResultDto>> CreateResult(int orderId, ResultCreateDto resultCreateDto)
        {
            var createdResult = await _resultService.CreateResultAsync(orderId, resultCreateDto);
            if (createdResult) { return NoContent(); }
            return BadRequest();
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResult(int id, ResultCreateDto resultCreateDto)
        {
            var updated = await _resultService.UpdateResultAsync(id, resultCreateDto);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var deleted = await _resultService.DeleteResultAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
