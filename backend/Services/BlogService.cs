using BlogApi.Models;
using System.Text.Json;

namespace BlogApi.Services
{
    public class BlogService
    {
        private readonly string _filePath;
        private readonly string _uploadPath;

        public BlogService()
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, "blogs.json");

            // 🔥 uploads folder path
            _uploadPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        // ✅ Read all blogs
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

        // ✅ Write blogs
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

        // ✅ Get blog by ID
        public Blog? GetById(int id)
        {
            return ReadData().FirstOrDefault(b => b.Id == id && b.IsActive);
        }

        // ✅ Create blog
        public Blog Create(Blog blog)
        {
            var blogs = ReadData();
            int newId = blogs.Any() ? blogs.Max(b => b.Id) + 1 : 1;

            blog.Id = newId;
            blog.IsUserCreated = true;
            blog.CreatedDate = DateTime.Now;
            blog.IsActive = true;

            // 🔥 fallback author
            if (string.IsNullOrWhiteSpace(blog.Author))
                blog.Author = "guest_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 🔥 Image fallback
            if (string.IsNullOrWhiteSpace(blog.Image))
                blog.Image = "https://via.placeholder.com/300x200";

            blogs.Add(blog);
            WriteData(blogs);

            return blog;
        }

        // ✅ Update blog
        public Blog? Update(int id, string title, string desc, string category, bool isActive, string? newImagePath = null)
        {
            var blogs = ReadData();
            var blog = blogs.FirstOrDefault(b => b.Id == id && b.IsActive);
            if (blog == null) return null;

            blog.Title = title ?? blog.Title;
            blog.Desc = desc ?? blog.Desc;
            blog.Category = category ?? blog.Category;
            blog.IsActive = isActive;

            // 🔥 Update image if provided
            if (!string.IsNullOrWhiteSpace(newImagePath))
            {
                // delete old image if uploaded
                if (!string.IsNullOrWhiteSpace(blog.Image) && blog.Image.StartsWith("/uploads"))
                {
                    var fullPath = Path.Combine(_uploadPath, Path.GetFileName(blog.Image));
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }

                blog.Image = newImagePath;
            }

            WriteData(blogs);
            return blog;
        }

        // ✅ Delete blog + image cleanup
        public bool Delete(int id, string? username = null)
        {
            var blogs = ReadData();
            var blog = blogs.FirstOrDefault(b => b.Id == id);

            if (blog == null) return false;
            if (!blog.IsUserCreated) return false;

            if (!string.IsNullOrWhiteSpace(username) && blog.Author != username)
                return false;

            // 🔥 Delete uploaded image
            if (!string.IsNullOrWhiteSpace(blog.Image) && blog.Image.StartsWith("/uploads"))
            {
                var fullPath = Path.Combine(_uploadPath, Path.GetFileName(blog.Image));
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }

            blog.IsActive = false;
            WriteData(blogs);

            return true;
        }
    }
}