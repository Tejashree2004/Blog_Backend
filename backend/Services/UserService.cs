using BlogApi.Models;
using System.Text.Json;
using Backend.Services;
using Microsoft.AspNetCore.Hosting;

namespace BlogApi.Services
{
    public class UserService
    {
        private readonly string _filePath;
        private readonly EmailService _emailService;

        public UserService(IWebHostEnvironment env, EmailService emailService)
        {
            _filePath = Path.Combine(env.ContentRootPath, "users.json");
            _emailService = emailService;
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

        // ===================== CREATE USER ===================== //
        public User? Create(User user, out string? error)
        {
            error = null;
            var users = ReadData();
            var email = user.Email.ToLower().Trim();

            if (users.Any(u => u.Email.ToLower().Trim() == email))
            {
                error = "User already exists with this email";
                return null;
            }

            int newId = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;

            user.Id = newId;
            user.Email = email;
            user.CreatedDate = DateTime.Now;
            user.IsVerified = false;
            user.Otp = null;

            users.Add(user);
            WriteData(users);

            return user;
        }

        // ===================== LOGIN ===================== //
        public User? Authenticate(string usernameOrEmail, string password)
        {
            var users = ReadData();
            var login = usernameOrEmail.ToLower().Trim();

            return users.FirstOrDefault(u =>
                (u.Email.ToLower().Trim() == login ||
                 (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Trim() == login)) &&
                 u.Password == password &&
                 u.IsVerified
            );
        }

        // ===================== GENERATE OTP ===================== //
        public string GenerateOtp(string email)
        {
            var users = ReadData();
            var user = users.FirstOrDefault(u => u.Email.ToLower().Trim() == email.ToLower().Trim());

            if (user == null)
                return "";

            // 🔥 If already verified → no OTP needed
            if (user.IsVerified)
            {
                Console.WriteLine("⚠️ User already verified");
                return "ALREADY_VERIFIED";
            }

            var otp = new Random().Next(100000, 999999).ToString();

            user.Otp = otp;
            WriteData(users);

            try
            {
_ = _emailService.SendEmailAsync(
    email,
    "Your OTP Code",
    otp
);


                Console.WriteLine($"✅ OTP sent to email: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Email send failed: {ex.Message}");
            }

            return otp;
        }

        // ===================== VERIFY OTP ===================== //
        public bool VerifyOtp(string email, string otp)
        {
            var users = ReadData();
            var user = users.FirstOrDefault(u => u.Email.ToLower().Trim() == email.ToLower().Trim());

            if (user == null)
            {
                Console.WriteLine("❌ User not found");
                return false;
            }

            // 🔥 Already verified case
            if (user.IsVerified)
            {
                Console.WriteLine("⚠️ User already verified");
                return true;
            }

            if (user.Otp == otp)
            {
                user.IsVerified = true;
                user.Otp = null;
                WriteData(users);

                Console.WriteLine("✅ OTP verified successfully");
                return true;
            }

            Console.WriteLine($"❌ Invalid OTP. Stored: {user.Otp}, Entered: {otp}");
            return false;
        }
    }
}