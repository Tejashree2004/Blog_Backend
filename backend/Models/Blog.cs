namespace BlogApi.Models
{
    public class Blog
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Desc { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public string Category { get; set; } = "blog";

        public bool IsUserCreated { get; set; } = false;

        // ⭐ IMPORTANT: This stores who created the blog (email or userId)
        // 👉 Use this to allow delete only for the owner
        public string Author { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}