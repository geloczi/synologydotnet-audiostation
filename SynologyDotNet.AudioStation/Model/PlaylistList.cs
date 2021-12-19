using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class PlaylistList : ListResponseBase
    {
        [JsonProperty("playlists")]
        public Playlist[] Playlists { get; set; }
    }
}
