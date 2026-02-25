using BlogApi.Models;
using System.Text.Json;

namespace BlogApi.Services
{
    public class UserService
    {
        private readonly string _filePath;

        public UserService(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "users.json");
        }

        private List<User> ReadData()
        {
            if (!File.Exists(_filePath))
                return new List<User>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        private void WriteData(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        // ✅ Create User
        public User Create(User user)
        {
            var users = ReadData();
            int newId = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            user.Id = newId;
            user.CreatedDate = DateTime.Now;
            users.Add(user);
            WriteData(users);
            return user;
        }

        // ✅ Authenticate User
        public User? Authenticate(string usernameOrEmail, string password)
        {
            return ReadData().FirstOrDefault(u =>
                (u.Username == usernameOrEmail || u.Email == usernameOrEmail) &&
                u.Password == password);
        }

        // ✅ Create Guest User
        public User CreateGuest()
        {
            var user = new User { Id = 0, Username = "Guest", IsGuest = true };
            return user;
        }
    }
}