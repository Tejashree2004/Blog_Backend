using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BlogApi.Models;
using BlogApi.Services;
using System.Linq;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly BlogService _blogService;
        private readonly SavedBlogService _savedService;

        public BlogsController(BlogService blogService, SavedBlogService savedService)
        {
            _blogService = blogService;
            _savedService = savedService;
        }

        // ===================== PUBLIC ===================== //

        // 🔥 GET ALL BLOGS
        [HttpGet]
        public IActionResult GetAll()
        {
            var blogs = _blogService.GetAll();
            return Ok(blogs);
        }

        // 🔥 GET MY BLOGS (JWT BASED - FIXED)
        [Authorize]
        [HttpGet("myblogs")]
        public IActionResult GetMyBlogs()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var blogs = _blogService.GetAll()
                .Where(b => b.Author == username)
                .ToList();

            return Ok(blogs);
        }

        // 🔥 GET FEED (OTHER USERS BLOGS)
        [Authorize]
        [HttpGet("feed")]
        public IActionResult GetFeed()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var blogs = _blogService.GetAll()
                .Where(b => b.Author != username && b.IsActive)
                .ToList();

            return Ok(blogs);
        }

        // 🔥 GET BLOG BY ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var blog = _blogService.GetById(id);
            if (blog == null || !blog.IsActive) return NotFound();

            return Ok(blog);
        }

        // ===================== PROTECTED ===================== //

        // 🔐 CREATE BLOG
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Blog newBlog)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Login required to create a blog." });
            }

            newBlog.Author = username;

            var blog = _blogService.Create(newBlog);

            return CreatedAtAction(nameof(Get), new { id = blog.Id }, blog);
        }

        // 🔐 DELETE BLOG
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var username = User.Identity?.Name;

            var blog = _blogService.GetById(id);

            if (blog == null)
                return NotFound(new { message = "Blog not found." });

            // ✅ Only owner can delete
            if (blog.Author != username)
                return Forbid();

            var success = _blogService.Delete(id);

            return Ok(new { message = "Blog deleted successfully." });
        }

        // 🔐 SAVE BLOG (FIXED - removed userId param)
        [Authorize]
        [HttpPost("save/{blogId}")]
        public IActionResult SaveBlog(int blogId)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var blog = _blogService.GetById(blogId);

            if (blog == null || !blog.IsActive)
                return NotFound("Blog not found.");

            _savedService.SaveBlog(username, blogId);

            return Ok(new { message = "Blog saved successfully." });
        }

        // 🔐 UNSAVE BLOG
        [Authorize]
        [HttpDelete("save/{blogId}")]
        public IActionResult UnsaveBlog(int blogId)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            _savedService.UnsaveBlog(username, blogId);

            return Ok(new { message = "Blog unsaved successfully." });
        }

        // 🔐 GET SAVED BLOGS
        [Authorize]
        [HttpGet("saved")]
        public IActionResult GetSavedBlogs()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var savedIds = _savedService.GetSavedBlogIds(username);

            var savedBlogs = savedIds
                .Select(id => _blogService.GetById(id))
                .Where(b => b != null && b.IsActive)
                .ToList();

            return Ok(savedBlogs);
        }
    }
}