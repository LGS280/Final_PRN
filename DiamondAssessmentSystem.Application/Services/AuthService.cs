using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<string> RegisterCustomerAsync(RegisterDto registerDto)
        {
            var newUser = _mapper.Map<User>(registerDto);

            newUser.UserType = "Customer";
            newUser.Status = "Active";

            var result = await _userRepository.RegisterCustomerAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Unable to create account: {errors}");
            }

            var roles = await _userRepository.GetUserRolesAsync(newUser);

            return "Registration successful";
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.ValidateUserCredentialsAsync(loginDto.Username, loginDto.Password);
            if (user == null)
                throw new UnauthorizedAccessException("Incorrect username or password.");

            var roles = await _userRepository.GetUserRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return new LoginResponseDto
            {
                Token = await token,
                Username = user.UserName,
                Roles = roles.ToList()
            };
        }

        private async Task<string> GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("UserType", user.UserType ?? "")
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var associatedId = await _userRepository.GetAssociatedIdByUserIdAsync(user.Id);
            if (!associatedId.HasValue)
            {
                throw new UnauthorizedAccessException("Your account is not assigned Customer or Employee. Please contact administrator.");
            }

            claims.Add(new Claim("AssociatedId", associatedId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
