using BlogApi.Models;
using System.Text.Json;

namespace BlogApi.Services
{
    public class SavedBlogService
    {
        private readonly string _filePath;

        public SavedBlogService(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "savedBlogs.json");
        }

        // ✅ Read saved blog IDs for a user
        private List<int> ReadSaved(int userId)
        {
            if (!File.Exists(_filePath)) return new List<int>();

            var json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<Dictionary<int, List<int>>>(json)
                       ?? new Dictionary<int, List<int>>();

            return data.ContainsKey(userId) ? data[userId] : new List<int>();
        }

        // ✅ Write saved blogs dictionary to file
        private void WriteSaved(Dictionary<int, List<int>> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        // ✅ Get saved blog IDs for a user
        public List<int> GetSavedBlogIds(int userId) => ReadSaved(userId);

        // ✅ Save a blog for a user
        public void SaveBlog(int userId, int blogId)
        {
            Dictionary<int, List<int>> data;

            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                data = JsonSerializer.Deserialize<Dictionary<int, List<int>>>(json)
                       ?? new Dictionary<int, List<int>>();
            }
            else
            {
                data = new Dictionary<int, List<int>>();
            }

            if (!data.ContainsKey(userId)) data[userId] = new List<int>();
            if (!data[userId].Contains(blogId)) data[userId].Add(blogId);

            WriteSaved(data);
        }

        // ✅ Unsave a blog for a user
        public void UnsaveBlog(int userId, int blogId)
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            var data = JsonSerializer.Deserialize<Dictionary<int, List<int>>>(json)
                       ?? new Dictionary<int, List<int>>();

            if (data.ContainsKey(userId))
            {
                data[userId].Remove(blogId);
                WriteSaved(data);
            }
        }
    }
}