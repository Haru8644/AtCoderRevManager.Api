namespace AtCoderRevManager.Api.Dtos
{
    public class CreateProblemDto
    {
        public string Id { get; set; }
        public string ContestId { get; set; }
        public string Title { get; set; }
        public int? Difficulty { get; set; }
    }
}