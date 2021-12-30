using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Id
    {
        [JsonProperty("id")]
        public string ID { get; set; }
    }
}
