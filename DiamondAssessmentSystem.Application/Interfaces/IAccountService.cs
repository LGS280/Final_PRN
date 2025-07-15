using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDto>> GetAllUsersAsync();
        Task<AccountDto> GetUserByIdAsync(string id);
        Task<AccountDto> CreateEmployeeAsync(RegisterEmployeesDto dto, string role);
        Task<bool> UpdateAccountAsync(string id, AccountDto accountDto);
        Task<bool> DeleteAccountAsync(string id);

    }
}
