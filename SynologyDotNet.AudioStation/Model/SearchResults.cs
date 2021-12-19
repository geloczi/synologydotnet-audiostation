using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class SearchResults
    {
        [JsonProperty("albumTotal")]
        public int AlbumTotal { get; set; }

        [JsonProperty("albums")]
        public Album[] Albums { get; set; }

        [JsonProperty("artistTotal")]
        public int ArtistTotal { get; set; }

        [JsonProperty("artists")]
        public Artist[] Artists { get; set; }

        [JsonProperty("songTotal")]
        public int SongTotal { get; set; }

        [JsonProperty("songs")]
        public Song[] Songs { get; set; }
    }
}
