using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using System.Threading.Tasks;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailService _emailService;

        public UsersController(UserService userService, EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        // ✅ Signup → create user, generate OTP, send email
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] User newUser)
        {
            if (newUser == null || string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Password))
                return BadRequest(new { message = "Email and Password are required." });

            if (string.IsNullOrWhiteSpace(newUser.Username))
                newUser.Username = newUser.Email.Split('@')[0];

            try
            {
                var createdUser = _userService.Create(newUser, out string? error);
                if (createdUser == null)
                    return BadRequest(new { message = error });

                var otp = _userService.GenerateOtp(createdUser.Email);

                // ✅ Send email safely (fail silently if SMTP fails)
                try
                {
                    if (!string.IsNullOrWhiteSpace(createdUser.Email) && _emailService != null)
                    {
                        await _emailService.SendEmailAsync(
                            createdUser.Email,
                            "Verify your email",
                            otp.ToString() // ✅ ONLY OTP (no "Your OTP is")
                        );
                    }
                }
                catch
                {
                    Console.WriteLine($"[WARN] Failed to send email OTP to {createdUser.Email}. OTP: {otp}");
                }

                return Ok(new
                {
                    message = "User created successfully. OTP sent to your email (or terminal).",
                    email = createdUser.Email
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Signup failed: " + ex.Message });
            }
        }

        // ✅ Verify Email OTP
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] VerifyRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest(new { message = "Email and OTP are required." });

            bool verified = _userService.VerifyOtp(request.Email, request.Otp);
            if (!verified)
                return BadRequest(new { message = "Invalid OTP." });

            return Ok(new { message = "Email verified successfully." });
        }

        // ✅ Login (flexible: email or username)
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            if (user == null || (string.IsNullOrWhiteSpace(user.Email) && string.IsNullOrWhiteSpace(user.Username)) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest(new { message = "Email/Username and Password are required." });

            // Use email if provided, else username
            string loginField = !string.IsNullOrWhiteSpace(user.Email) ? user.Email : user.Username;

            var authUser = _userService.Authenticate(loginField, user.Password);

            if (authUser == null)
                return Unauthorized(new { message = "Invalid credentials or email not verified." });

            return Ok(new
            {
                username = authUser.Username,
                email = authUser.Email
            });
        }
    }
}