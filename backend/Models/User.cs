namespace BlogApi.Models
{
    public class User
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsGuest { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsVerified { get; set; } = false;

        // 🔥 OTP field
        public string? Otp { get; set; } = null;
    }
}