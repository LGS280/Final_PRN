using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: api/Account
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var accountDtos = await _accountService.GetAllUsersAsync();
            return Ok(accountDtos);
        }

        // GET: api/Account/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            var accountDto = await _accountService.GetUserByIdAsync(id);
            if (accountDto == null)
            {
                return NotFound();
            }

            return Ok(accountDto);
        }

        [HttpPost("RegisterEmployee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeesDto registerDto)
        {
            var role = registerDto.Role;

            if (string.IsNullOrEmpty(role))
                return BadRequest("Roles cannot be left empty.");

            try
            {
                var account = await _accountService.CreateEmployeeAsync(registerDto, role);
                return CreatedAtAction(nameof(RegisterEmployee), account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Account/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, AccountDto accountDto)
        {
            if (id != accountDto.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            var updated = await _accountService.UpdateAccountAsync(id, accountDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Account/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var deleted = await _accountService.DeleteAccountAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
