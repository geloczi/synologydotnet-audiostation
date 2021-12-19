using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class PlaylistList : ListResponseBase
    {
        public Playlist[] playlists { get; set; }
    }
}
