
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // ✅ Add this
using BlogApi.Models;
using BlogApi.Services;

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

        // 🔥 GET ALL ACTIVE BLOGS (Public)
        [HttpGet]
        public IActionResult GetAll()
        {
            var blogs = _blogService.GetAll();
            return Ok(blogs);
        }

        // 🔥 GET ONLY MY BLOGS (Public)
        [HttpGet("myblogs")]
        public IActionResult GetMyBlogs()
        {
            var blogs = _blogService.GetBlogsOnly();
            return Ok(blogs);
        }

        // 🔥 GET ONLY FEED (Public)
        [HttpGet("feed")]
        public IActionResult GetFeed()
        {
            var blogs = _blogService.GetFeedOnly();
            return Ok(blogs);
        }

        // 🔥 GET BLOG BY ID (Public)
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var blog = _blogService.GetById(id);
            if (blog == null || !blog.IsActive) return NotFound();
            return Ok(blog);
        }

        // 🔐 CREATE BLOG (Protected)
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Blog newBlog)
        {
            var blog = _blogService.Create(newBlog);
            return CreatedAtAction(nameof(Get), new { id = blog.Id }, blog);
        }

        // 🔐 DELETE BLOG (Protected)
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _blogService.Delete(id);
            if (!success)
                return NotFound(new { message = "Blog not found or cannot be deleted." });

            return Ok(new { message = "Blog deleted successfully." });
        }

        // 🔐 SAVE A BLOG
        [Authorize]
        [HttpPost("save/{blogId}")]
        public IActionResult SaveBlog(int blogId, [FromQuery] int userId)
        {
            var blog = _blogService.GetById(blogId);
            if (blog == null || !blog.IsActive) 
                return NotFound("Blog not found or inactive.");

            _savedService.SaveBlog(userId, blogId);
            return Ok(new { message = "Blog saved successfully." });
        }

        // 🔐 UNSAVE A BLOG
        [Authorize]
        [HttpDelete("save/{blogId}")]
        public IActionResult UnsaveBlog(int blogId, [FromQuery] int userId)
        {
            _savedService.UnsaveBlog(userId, blogId);
            return Ok(new { message = "Blog unsaved successfully." });
        }

        // 🔐 GET SAVED BLOGS
        [Authorize]
        [HttpGet("saved")]
        public IActionResult GetSavedBlogs([FromQuery] int userId)
        {
            var savedIds = _savedService.GetSavedBlogIds(userId);
            var savedBlogs = savedIds
                .Select(id => _blogService.GetById(id))
                .Where(b => b != null && b.IsActive)
                .ToList();

            return Ok(savedBlogs);
        }
    }
}
