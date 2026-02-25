using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
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

        // ✅ Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var authUser = _userService.Authenticate(user.Username, user.Password);
            if (authUser == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(authUser);
        }

        // ✅ Guest Login
        [HttpPost("guest")]
        public IActionResult GuestLogin()
        {
            var guest = _userService.CreateGuest();
            return Ok(guest);
        }
    }
}