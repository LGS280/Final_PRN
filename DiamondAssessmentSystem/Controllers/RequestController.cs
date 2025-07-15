using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly ICurrentUserService _currentUser;

        public RequestController(IRequestService requestService, ICurrentUserService currentUser)
        {
            _requestService = requestService;
            _currentUser = currentUser;
        }

        // GET: api/request
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetAllRequests()
        {
            var requests = await _requestService.GetRequestsAsync();
            return Ok(requests);
        }

        // GET: api/request/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestDto>> GetRequestById(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(request);
        }

        // GET: api/request/my
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<RequestDto>>> GetMyRequests()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var requests = await _requestService.GetRequestsByCustomerIdAsync(userId);
            return Ok(requests);
        }

        // POST: api/request/draft
        [HttpPost("draft")]
        public async Task<ActionResult<RequestDto>> CreateDraft([FromBody] RequestCreateDto draftDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _requestService.CreateRequestForCustomerAsync(userId, draftDto, "Draft");
            if (created == null)
                return BadRequest("Could not create draft request.");

            return CreatedAtAction(nameof(GetRequestById), new { id = created.RequestId }, created);
        }

        // POST: api/request/submit
        [HttpPost("submit")]
        public async Task<ActionResult<RequestDto>> CreateRequest([FromBody] RequestCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _requestService.CreateRequestForCustomerAsync(userId, createDto, "Pending");
            if (created == null)
                return BadRequest("Could not create request.");

            return CreatedAtAction(nameof(GetRequestById), new { id = created.RequestId }, created);
        }

        // POST: api/request/{id}/cancel
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _requestService.CancelRequestAsync(userId, id);
            if (!success)
                return BadRequest("Cancellation is only allowed for 'Draft' status.");

            return Ok();
        }

        // PUT: api/request/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(int id, [FromBody] RequestCreateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _requestService.UpdateRequestAsync(id, updateDto);
            if (!updated)
                return NotFound();

            return NoContent();
        }
    }
}
