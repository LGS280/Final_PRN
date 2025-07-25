using DiamondAssessmentSystem.Application.Email;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly UserManager<User> _userManager;


        public EmailService(IOptions<EmailSettings> emailSettings, UserManager<User> userManager)
        {
            _emailSettings = emailSettings.Value;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(toEmail));
            message.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;

            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                client.EnableSsl = true;

                await client.SendMailAsync(message);
            }
        }
    }
}
