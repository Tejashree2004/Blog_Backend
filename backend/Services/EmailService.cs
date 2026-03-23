using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required", nameof(toEmail));

            try
            {
                var emailSettings = _config.GetSection("SMTP");

                // 👉 Email config (can stay in appsettings.json)
                string senderEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL")
                    ?? emailSettings["Email"]
                    ?? throw new Exception("SMTP Email not configured");

                // 🔥 Password ONLY from .env (secure)
                string senderPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                    ?? throw new Exception("SMTP Password not configured in .env");

                string smtpHost = emailSettings["Host"] ?? "smtp.gmail.com";

                int smtpPort = 587;
                if (!int.TryParse(emailSettings["Port"], out smtpPort))
                {
                    smtpPort = 587;
                }

                // ✅ Create email
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Blog App", senderEmail));
                emailMessage.To.Add(MailboxAddress.Parse(toEmail));
                emailMessage.Subject = string.IsNullOrWhiteSpace(subject) ? "OTP Verification" : subject;

                // 🔥 PROFESSIONAL EMAIL BODY (ONLY CHANGE)
                var currentTime = DateTime.Now.ToString("f");

                string emailBody = $@"
Hello,

We received a request to verify your email address for your Blog App account.

 {message}

This OTP is valid for 10 minutes.

Request Time: {currentTime}

If you did not request this, please ignore this email and ensure your account is secure.

Regards,  
Blog App Team
";

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = emailBody
                };

                // ✅ Send email
                using var client = new SmtpClient();

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

                // 🔐 Secure authentication
                await client.AuthenticateAsync(senderEmail, senderPassword);

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);

                Console.WriteLine($"✅ Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Email send failed: " + ex.Message);
                throw;
            }
        }
    }
}