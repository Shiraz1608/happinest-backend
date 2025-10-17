using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Happinest.Models;
using Happinest.Services.AuthAPI.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace HappinestApi.Services
{/// <summary>
 /// Service class responsible for handling email-related operations, such as retrieving email templates.
 /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        private readonly ITokenService _tokenService;
        public EmailService(IConfiguration configuration,
                           ITokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml, string displayName = null, string? fromEmail = null, string replyToEmail = null)
        {
            try
            {
                // Load SMTP settings from configuration
                var smtpHost = _configuration["Host"];
                var smtpPort = Convert.ToInt32(_configuration["EmailPort"]);
                var smtpUser = _configuration["SenderEmail"];
                var smtpPass = _configuration["SenderPassword"];
                var senderEmail = fromEmail ?? smtpUser;
                var enableSsl = Convert.ToBoolean(_configuration["EnableSsl"]);

                using var smtpClient = new SmtpClient
                {
                    Host = smtpHost,
                    Port = smtpPort,
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, displayName),
                    Subject = subject ?? "No Subject",
                    Body = bodyHtml,
                    IsBodyHtml = true,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8
                };
                // Add ReplyTo
                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    mailMessage.ReplyToList.Add(new MailAddress(replyToEmail));
                }
                // Add HTML body
                var htmlView = AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, "text/Html");

                // (Optional) Add plain text fallback
                var plainView = AlternateView.CreateAlternateViewFromString(
                    System.Text.RegularExpressions.Regex.Replace(bodyHtml, "<.*?>", string.Empty),
                    Encoding.UTF8,
                    "text/plain"
                );

                mailMessage.AlternateViews.Add(plainView);
                mailMessage.AlternateViews.Add(htmlView);

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = new StringBuilder()
                                    .Append("Failed to send email to: ")
                                    .Append(toEmail)
                                    .Append(". Error: ")
                                    .Append(ex.InnerException?.Message ?? ex?.Message)
                                    .ToString();

                //_cloudwatchlogger.LogError(errorMessage);

                throw;
            }
        }

        
    }
}
