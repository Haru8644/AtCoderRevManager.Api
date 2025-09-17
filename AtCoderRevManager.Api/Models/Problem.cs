namespace AtCoderRevManager.Api.Models
{
    public class Problem
    {
        public string Id { get; set; }
        public string ContestId { get; set; }
        public string Title { get; set; }
        public int? Difficulty { get; set; }
    }
}