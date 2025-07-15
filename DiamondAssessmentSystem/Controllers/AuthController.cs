using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var loginResponse = await _authService.LoginAsync(loginDto);
                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "System error: " + ex.Message });
            }
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDto registerDto)
        {
            try
            {
                var message = await _authService.RegisterCustomerAsync(registerDto);
                return Created(string.Empty, new { message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "System error: " + ex.Message });
            }
        }


        //// POST: api/Auth/RegisterAdmin
        //[HttpPost("RegisterAdmin")]
        //public async Task<ActionResult<AccountDto>> RegisterAdmin([FromBody] AccountDto registerDto)
        //{
        //    try
        //    {
        //        var accountDto = await _authService.RegisterAdminAsync(registerDto);
        //        return Created(string.Empty, accountDto);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Lỗi hệ thống: " + ex.Message });
        //    }
        //}
    }
}
