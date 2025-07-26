using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Email;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IEmailService emailService,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
            _userManager = userManager;
            _mapper = mapper;
        }

        //public async Task<string> RegisterCustomerAsync(RegisterDto registerDto)
        //{
        //    var newUser = _mapper.Map<User>(registerDto);

        //    newUser.UserType = "Customer";
        //    newUser.Status = "Active";
        //    newUser.DateCreated = DateTime.Now;

        //    var result = await _userRepository.RegisterCustomerAsync(newUser, registerDto.Password, registerDto.Email);
        //    if (!result.Succeeded)
        //    {
        //        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        //        throw new Exception($"Unable to create account: {errors}");
        //    }

        //    // Generate email confirmation token
        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

        //    // Encode token for URL
        //    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        //    // Tạo URL xác nhận
        //    //var confirmationUrl = $"https://localhost:7278/api/Email/confirm?userId={newUser.Id}&token={encodedToken}";
        //    var confirmationUrl = $"https://localhost:7278/Email/confirm?userId={newUser.Id}&token={encodedToken}";

        //    var emailBody = EmailTemplates.ConfirmEmailTemplate(confirmationUrl);

        //    await _emailService.SendEmailAsync(newUser.Email, "Confirm your email", emailBody);

        //    return "Registration successful. Please check your email to confirm your account.";
        //}

        public async Task<User> RegisterCustomerAsync(RegisterDto registerDto)
        {
            var newUser = _mapper.Map<User>(registerDto);

            newUser.UserType = "Customer";
            newUser.Status = "Active";
            newUser.DateCreated = DateTime.Now;

            var result = await _userRepository.RegisterCustomerAsync(newUser, registerDto.Password, registerDto.Email);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Unable to create account: {errors}");
            }

            return newUser;
        }

        public async Task SendConfirmationEmailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationUrl = $"https://0e9f9bbc75d8.ngrok-free.app/Auth/ConfirmEmail?userId={user.Id}&token={encodedToken}";
            var emailBody = EmailTemplates.ConfirmEmailTemplate(confirmationUrl);

            await _emailService.SendEmailAsync(user.Email, "Confirm your email", emailBody);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password);
            if (user == null)
                throw new UnauthorizedAccessException("Incorrect email or password.");

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new Exception("You must confirm your email to log in.");
            }
            if (!user.EmailConfirmed)
            {
                throw new UnauthorizedAccessException("Please confirm your email before logging in.");
            }

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
            if (associatedId.HasValue)
            {
                claims.Add(new Claim("AssociatedId", associatedId.Value.ToString()));
            }

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
