using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace BlogApi.Services
{
    public class SavedBlogService
    {
        private readonly string _filePath;
        private readonly object _fileLock = new();

        public SavedBlogService(IWebHostEnvironment env)
        {
            // content root — fallback to AppContext.BaseDirectory if env null
            var contentRoot = env?.ContentRootPath ?? AppContext.BaseDirectory;
            _filePath = Path.Combine(contentRoot, "savedBlogs.json");

            // Ensure directory and file exist (best effort)
            try
            {
                var dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(_filePath))
                    File.WriteAllText(_filePath, "{}");
            }
            catch
            {
                // ignore — will be handled during read/write operations
            }
        }

        // Read full dictionary from file (safe)
        private Dictionary<string, List<int>> ReadAll()
        {
            lock (_fileLock)
            {
                try
                {
                    if (!File.Exists(_filePath))
                        return new Dictionary<string, List<int>>();

                    var json = File.ReadAllText(_filePath);
                    if (string.IsNullOrWhiteSpace(json))
                        return new Dictionary<string, List<int>>();

                    var data = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(
                        json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return data ?? new Dictionary<string, List<int>>();
                }
                catch
                {
                    // If file is corrupted or cannot be parsed, return empty structure
                    return new Dictionary<string, List<int>>();
                }
            }
        }

        // Write full dictionary to file (safe)
        private void WriteAll(Dictionary<string, List<int>> data)
        {
            lock (_fileLock)
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }

        // Public: get saved blog ids for a user
        public List<int> GetSavedBlogIds(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new List<int>();

            var data = ReadAll();
            if (data.ContainsKey(userId))
                return new List<int>(data[userId]); // return copy

            return new List<int>();
        }

        // Public: save blog for a user (idempotent)
        public void SaveBlog(string userId, int blogId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("userId is required", nameof(userId));

            var data = ReadAll();

            if (!data.ContainsKey(userId))
                data[userId] = new List<int>();

            if (!data[userId].Contains(blogId))
            {
                data[userId].Add(blogId);
                WriteAll(data);
            }
            // if already exists — do nothing (idempotent)
        }

        // Public: unsave blog for a user
        public void UnsaveBlog(string userId, int blogId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return;

            var data = ReadAll();

            if (data.ContainsKey(userId) && data[userId].Contains(blogId))
            {
                data[userId].Remove(blogId);
                WriteAll(data);
            }
        }
    }
}