using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IAuthService
    {
        //Task<String> RegisterCustomerAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);

        Task<User> RegisterCustomerAsync(RegisterDto registerDto);

        Task SendConfirmationEmailAsync(User user);

    }
}
