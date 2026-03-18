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

                string senderEmail = emailSettings["Email"]
                    ?? throw new Exception("SMTP Email not configured");

                string senderPassword = emailSettings["Password"]
                    ?? throw new Exception("SMTP Password not configured");

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

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = message ?? "Your OTP code"
                };

                // ✅ Send email
                using var client = new SmtpClient();

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

                // 🔥 IMPORTANT for Gmail
                await client.AuthenticateAsync(senderEmail, senderPassword);

                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);

                Console.WriteLine($"✅ Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Email send failed: " + ex.Message);
                throw; // 🔥 so you can see real error in console
            }
        }
    }
}