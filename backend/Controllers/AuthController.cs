using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(UserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        // ===================== SIGNUP ===================== //
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] User newUser)
        {
            if (newUser == null ||
                string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.Username) ||
                string.IsNullOrWhiteSpace(newUser.Password))
            {
                return BadRequest(new { message = "Email, Username, and Password are required." });
            }

            try
            {
                string? error;
                var createdUser = _userService.Create(newUser, out error);

                if (createdUser == null)
                    return BadRequest(new { message = error });

                // ✅ Generate OTP
                _userService.GenerateOtp(createdUser.Email);

                return Ok(new
                {
                    message = "User created successfully. OTP sent to email.",
                    email = createdUser.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Signup failed: " + ex.Message });
            }
        }

        // ===================== LOGIN ===================== //
        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginRequest)
        {
            if (loginRequest == null ||
                string.IsNullOrWhiteSpace(loginRequest.Email) ||
                string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            try
            {
                // 🔍 Authenticate user
                var user = _userService.Authenticate(loginRequest.Email, loginRequest.Password);

                if (user == null)
                {
                    return Unauthorized(new
                    {
                        message = "Invalid credentials or email not verified."
                    });
                }

                // 🔥 GENERATE JWT TOKEN
                var token = _jwtHelper.GenerateToken(user.Username);

                // ✅ RETURN EVERYTHING FRONTEND NEEDS
                return Ok(new
                {
                    token = token,
                    username = user.Username,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Login failed: " + ex.Message
                });
            }
        }

        // ===================== VERIFY EMAIL ===================== //
        [HttpPost("verify-email")]
        public IActionResult VerifyEmail([FromBody] VerifyRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new { message = "Email and OTP are required." });
            }

            try
            {
                bool verified = _userService.VerifyOtp(request.Email, request.Otp);

                if (!verified)
                {
                    return BadRequest(new { message = "Invalid OTP." });
                }

                return Ok(new
                {
                    message = "Email verified successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Verification failed: " + ex.Message
                });
            }
        }
    }
}