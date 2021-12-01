using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class FileTags : ApiResponse
    {
        [JsonProperty("files")]
        public FileTag[] Files { get; set; }
        [JsonProperty("lyrics")]
        public string Lyrics { get; set; }
        [JsonProperty("read_fail_count")]
        public int ReadFailCount { get; set; }
    }
    
    public class FileTag
    {
        [JsonProperty("album")]
        public string Album { get; set; }
        [JsonProperty("album_artist")]
        public string AlbumArtist { get; set; }
        [JsonProperty("artist")]
        public string Artist { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("composer")]
        public string Composer { get; set; }
        [JsonProperty("disc")]
        public int Disc { get; set; }
        [JsonProperty("genre")]
        public string Genre { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("track")]
        public int Track { get; set; }
        [JsonProperty("year")]
        public int Year { get; set; }
    }
}
