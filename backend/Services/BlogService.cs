using BlogApi.Models;
using System.Text.Json;

namespace BlogApi.Services
{
    public class BlogService
    {
        private readonly string _filePath;

        public BlogService(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "blogs.json");
        }

        private List<Blog> ReadData()
        {
            if (!File.Exists(_filePath))
                return new List<Blog>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Blog>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Blog>();
        }

        private void WriteData(List<Blog> blogs)
        {
            var json = JsonSerializer.Serialize(blogs, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }

        // ✅ GET ALL ACTIVE BLOGS
        public List<Blog> GetAll()
        {
            return ReadData().Where(b => b.IsActive).ToList();
        }

        // ✅ GET ONLY MY BLOGS (category = blog)
        public List<Blog> GetBlogsOnly()
        {
            return ReadData()
                .Where(b => b.Category?.ToLower() == "blog" && b.IsActive)
                .ToList();
        }

        // ✅ GET ONLY FEED (category = feed)
        public List<Blog> GetFeedOnly()
        {
            return ReadData()
                .Where(b => b.Category?.ToLower() == "feed" && b.IsActive)
                .ToList();
        }

        public Blog? GetById(int id)
        {
            return ReadData().FirstOrDefault(b => b.Id == id && b.IsActive);
        }

        public Blog Create(Blog blog)
        {
            var blogs = ReadData();

            int newId = blogs.Count > 0 ? blogs.Max(b => b.Id) + 1 : 1;
            blog.Id = newId;
            blog.IsUserCreated = true;
            blog.CreatedDate = DateTime.Now;
            blog.IsActive = true;

            blogs.Add(blog);
            WriteData(blogs);

            return blog;
        }

        // ✅ Soft delete only user-created blogs
        public bool Delete(int id)
        {
            var blogs = ReadData();
            var blog = blogs.FirstOrDefault(b => b.Id == id);

            if (blog == null)
                return false; // blog not found

            if (!blog.IsUserCreated)
                return false; // 🔒 static blogs cannot be deleted

            blog.IsActive = false;
            WriteData(blogs);
            return true;
        }
    }
}