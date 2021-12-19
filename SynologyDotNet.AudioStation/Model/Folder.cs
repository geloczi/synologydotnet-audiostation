using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Folder
    {
        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("is_personal")]
        public bool IsPersonal { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }

        public override string ToString() => Path ?? base.ToString();
    }
}
