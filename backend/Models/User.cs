namespace BlogApi.Models
{
    public class User
    {
        public int Id { get; set; }                    // Unique ID
        public string Username { get; set; } = "";     // Username for login
        public string Email { get; set; } = "";        // Email for signup
        public string Password { get; set; } = "";     // Password (plain text for demo; later hash recommended)
        public bool IsGuest { get; set; } = false;     // Guest user
        public DateTime CreatedDate { get; set; } = DateTime.Now;  // Account creation date
    }
}