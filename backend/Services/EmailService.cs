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

                string senderEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL")
                    ?? emailSettings["Email"]
                    ?? throw new Exception("SMTP Email not configured");

                string senderPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                    ?? throw new Exception("SMTP Password not configured in .env");

                string smtpHost = emailSettings["Host"] ?? "smtp.gmail.com";

                // ✅ Dual port fallback: 465 SSL default, fallback to 587 STARTTLS
                int smtpPort = 465; 
                SecureSocketOptions sslOption = SecureSocketOptions.SslOnConnect;

                if (!int.TryParse(emailSettings["Port"], out smtpPort))
                {
                    smtpPort = 465;
                    sslOption = SecureSocketOptions.SslOnConnect;
                }
                else
                {
                    // Optional: if port in config is 587
                    if (smtpPort == 587)
                        sslOption = SecureSocketOptions.StartTls;
                }

                // ✅ Create email
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Blog App", senderEmail));
                emailMessage.To.Add(MailboxAddress.Parse(toEmail));
                emailMessage.Subject = string.IsNullOrWhiteSpace(subject) ? "OTP Verification" : subject;

                var currentTime = DateTime.Now.ToString("f");

                // 🔹 HTML email body with bold OTP
                string emailBody = $@"
<html>
<body>
<p>Hello,</p>

<p>Here’s your secure code to access your Blog App account:</p>

<p><b>{message}</b></p>

<p>This code is valid for 10 minutes.</p>

<p>Request Time: {currentTime}</p>

<p>If you did not request this, please ignore this email and ensure your account is secure.</p>

<p>Regards,<br/>Blog App Team</p>
</body>
</html>
";

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = emailBody
                };

                using var client = new SmtpClient();

                try
                {
                    // 🔹 Try default SSL connection (465)
                    await client.ConnectAsync(smtpHost, smtpPort, sslOption);
                }
                catch
                {
                    // 🔹 If fails, fallback to 587 STARTTLS
                    smtpPort = 587;
                    sslOption = SecureSocketOptions.StartTls;
                    await client.ConnectAsync(smtpHost, smtpPort, sslOption);
                }

                // 🔹 Authenticate
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