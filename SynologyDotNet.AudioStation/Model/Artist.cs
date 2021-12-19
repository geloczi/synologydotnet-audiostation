using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Artist
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("additional")]
        public GenreAdditional Additional { get; set; }

        public override string ToString() => Name ?? base.ToString();
    }

    public struct ArtistAdditional
    {
        [JsonProperty("avg_rating")]
        public AvgRating AverageRating { get; set; }
    }
}
