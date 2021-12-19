using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Composer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("additional")]
        public ComposerAdditional Additional { get; set; }

        public override string ToString() => Name ?? base.ToString();
    }

    public struct ComposerAdditional
    {
        [JsonProperty("avg_rating")]
        public ComposerRating AverageRating { get; set; }
    }

    public struct ComposerRating
    {
        [JsonProperty("rating")]
        public int Value { get; set; }
    }
}
