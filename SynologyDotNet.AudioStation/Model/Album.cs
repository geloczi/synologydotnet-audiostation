using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class Album
    {
        [JsonProperty("album_artist")]
        public string AlbumArtist { get; set; }
        [JsonProperty("artist")]
        public string Artist { get; set; }
        [JsonProperty("display_artist")]
        public string DisplayArtist { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("year")]
        public int Year { get; set; }
        [JsonProperty("additional")]
        public AlbumAdditional Additional { get; set; }

        public override string ToString() => Name ?? base.ToString();

    }
    
    public struct AlbumAdditional
    {
        [JsonProperty("avg_rating")]
        public AverageRating AverageRating { get; set; }
    }

    public struct AverageRating
    {
        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
}
