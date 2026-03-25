namespace BlogApi.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Desc { get; set; } = string.Empty;

        // 🔥 IMAGE PATH (stored after upload)
        public string Image { get; set; } = string.Empty;

        public string Category { get; set; } = "blog";

        public bool IsUserCreated { get; set; } = false;

        // 🔐 Owner of blog
        public string Author { get; set; } = string.Empty;

        // 🔥 BETTER DATE FORMAT (optional but good)
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}