namespace AtCoderRevManager.Api.Dtos
{
    public class CreateReviewItemDto
    {
        public string ProblemId { get; set; }
        public string? Comment { get; set; }
    }
}