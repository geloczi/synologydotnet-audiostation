using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Playlist
    {
        public static class ReservedNames
        {
            public const string SharedSongs = "__SYNO_AUDIO_SHARED_SONGS__";
        }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("library")]
        public string Library { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("sharing_status")]
        public string SharingStatus { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("additional")]
        public PlaylistAdditional Additional { get; set; }

        public override string ToString() => ID ?? base.ToString();
    }

    public struct PlaylistAdditional
    {
        [JsonProperty("songs")]
        public Song[] Songs { get; set; }

        [JsonProperty("songs_offset")]
        public int Offset { get; set; }

        [JsonProperty("songs_total")]
        public int Total { get; set; }
    }
}
