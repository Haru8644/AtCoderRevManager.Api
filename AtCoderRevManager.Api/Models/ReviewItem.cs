namespace AtCoderRevManager.Api.Models
{
    public class ReviewItem
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ProblemId { get; set; }
        public string? Comment { get; set; }
        public DateTime NextReviewAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}