using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public struct AvgRating
    {
        [JsonProperty("rating")]
        public double Value { get; set; }
    }
}
