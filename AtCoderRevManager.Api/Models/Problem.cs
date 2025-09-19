using System.Text.Json.Serialization;

namespace AtCoderRevManager.Api.Models
{
    public class Problem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("contest_id")]
        public string ContestId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("difficulty")]
        public int? Difficulty { get; set; }
    }
}