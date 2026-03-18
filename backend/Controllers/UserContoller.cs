using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Helpers; // ✅ Add this
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper; // ✅ Add this

        public UsersController(UserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        // ✅ Signup
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] User newUser)
        {
            var users = _userService.Authenticate(newUser.Username, newUser.Password);
            if (users != null)
                return BadRequest(new { message = "User already exists." });

            var createdUser = _userService.Create(newUser);
            return Ok(createdUser);
        }

        // ✅ Login (Now returns JWT)
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var authUser = _userService.Authenticate(user.Username, user.Password);
            if (authUser == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // 🔐 Generate JWT Token
            var token = _jwtHelper.GenerateToken(authUser.Username);

            return Ok(new
            {
                token = token,
                username = authUser.Username
            });
        }

        // ✅ Guest Login
        [HttpPost("guest")]
        public IActionResult GuestLogin()
        {
            var guest = _userService.CreateGuest();

            // Optional: give guest token also
            var token = _jwtHelper.GenerateToken(guest.Username);

            return Ok(new
            {
                token = token,
                username = guest.Username
            });
        }
    }
}