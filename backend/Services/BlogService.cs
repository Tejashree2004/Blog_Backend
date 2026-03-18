using BlogApi.Models;
using System.Text.Json;

namespace BlogApi.Services
{
    public class BlogService
    {
        private readonly string _filePath;

        public BlogService()
        {
            // ✅ Use Path.Combine for cross-platform safety
            _filePath = Path.Combine(AppContext.BaseDirectory, "blogs.json");
        }

        // ✅ Read all blogs from JSON file
        private List<Blog> ReadData()
        {
            if (!File.Exists(_filePath))
                return new List<Blog>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Blog>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Blog>();
        }

        // ✅ Write blogs to JSON file
        private void WriteData(List<Blog> blogs)
        {
            var json = JsonSerializer.Serialize(blogs, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_filePath, json);
        }

        // ✅ Get all active blogs
        public List<Blog> GetAll()
        {
            return ReadData().Where(b => b.IsActive).ToList();
        }

        // ✅ Get only user-created blogs (category = "blog")
        public List<Blog> GetBlogsOnly(string? username = null)
        {
            var blogs = ReadData()
                        .Where(b => b.Category?.ToLower() == "blog" && b.IsActive)
                        .ToList();

            if (!string.IsNullOrWhiteSpace(username))
            {
                blogs = blogs.Where(b => b.Author == username).ToList();
            }

            return blogs;
        }

        // ✅ Get feed blogs (category = "feed")
        public List<Blog> GetFeedOnly()
        {
            return ReadData()
                .Where(b => b.Category?.ToLower() == "feed" && b.IsActive)
                .ToList();
        }

        // ✅ Get blog by ID
        public Blog? GetById(int id)
        {
            return ReadData().FirstOrDefault(b => b.Id == id && b.IsActive);
        }

        // ✅ Create a new blog
        public Blog Create(Blog blog)
        {
            var blogs = ReadData();

            int newId = blogs.Count > 0 ? blogs.Max(b => b.Id) + 1 : 1;
            blog.Id = newId;
            blog.IsUserCreated = true;
            blog.CreatedDate = DateTime.Now;
            blog.IsActive = true;

            // ✅ Default author for guest users
            if (string.IsNullOrWhiteSpace(blog.Author))
            {
                blog.Author = "guest_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            blogs.Add(blog);
            WriteData(blogs);

            return blog;
        }

        // ✅ Soft delete a user-created blog
        public bool Delete(int id, string? username = null)
        {
            var blogs = ReadData();
            var blog = blogs.FirstOrDefault(b => b.Id == id);

            if (blog == null) return false; // not found
            if (!blog.IsUserCreated) return false; // cannot delete static blogs

            // ✅ Only allow owner to delete
            if (!string.IsNullOrWhiteSpace(username) && blog.Author != username)
                return false;

            blog.IsActive = false;
            WriteData(blogs);
            return true;
        }
    }
}