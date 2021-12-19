using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Genre
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("additional")]
        public GenreAdditional Additional { get; set; }

        public override string ToString() => Name ?? base.ToString();
    }

    public struct GenreAdditional
    {
        [JsonProperty("avg_rating")]
        public AvgRating AverageRating { get; set; }
    }
}
