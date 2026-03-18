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

        // ⭐ ADD THIS (MOST IMPORTANT)
        public string Author { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}