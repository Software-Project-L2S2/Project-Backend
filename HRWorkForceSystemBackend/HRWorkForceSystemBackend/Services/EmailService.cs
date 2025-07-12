using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace HRWorkForceSystemBackend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpSection = _config.GetSection("SmtpSettings");
                string host = smtpSection["Host"];
                string username = smtpSection["Username"];
                string password = smtpSection["Password"];
                string portStr = smtpSection["Port"];
                string enableSslStr = smtpSection["EnableSSL"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception("SMTP configuration is missing required fields.");

                int port = int.TryParse(portStr, out int parsedPort) ? parsedPort : 587;
                bool enableSsl = bool.TryParse(enableSslStr, out bool parsedSSL) ? parsedSSL : true;

                using var smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP error occurred while sending email to {Email}", toEmail);
                throw new Exception("Failed to send email. SMTP error: " + smtpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while sending email to {Email}", toEmail);
                throw new Exception("Failed to send email. Error: " + ex.Message);
            }
        }
    }
}
