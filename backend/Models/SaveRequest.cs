namespace BlogApi.Models
{
    // Rename class to match controller/service usage
    public class SaveRequest
    {
        public string UserId { get; set; } = string.Empty; // string as per controller
        public int BlogId { get; set; }
    }
}