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
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }

        // ✅ Create User (Signup)
        public User Create(User user)
        {
            var users = ReadData();

            // Normalize email
            var email = user.Email.ToLower().Trim();

            // Check if email already exists
            if (users.Any(u => u.Email.ToLower().Trim() == email))
            {
                throw new Exception("User already exists with this email");
            }

            int newId = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;

            user.Id = newId;
            user.Email = email;
            user.CreatedDate = DateTime.Now;

            users.Add(user);
            WriteData(users);

            return user;
        }

        // ✅ Authenticate User (Login)
        public User? Authenticate(string usernameOrEmail, string password)
        {
            var users = ReadData();

            var login = usernameOrEmail.ToLower().Trim();

            return users.FirstOrDefault(u =>
                (u.Email.ToLower().Trim() == login ||
                 (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Trim() == login))
                && u.Password == password
            );
        }

        // ✅ Create Guest User
        public User CreateGuest()
        {
            return new User
            {
                Id = 0,
                Username = "Guest",
                IsGuest = true
            };
        }
    }
}