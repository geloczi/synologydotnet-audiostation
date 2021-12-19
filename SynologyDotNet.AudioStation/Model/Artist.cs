using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Artist
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public override string ToString() => Name ?? base.ToString();
    }
}
