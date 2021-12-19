namespace SynologyDotNet.AudioStation.Model
{
    public class Playlist
    {
        public static class ReservedNames
        {
            public const string SharedSongs = "__SYNO_AUDIO_SHARED_SONGS__";
        }

        public string id { get; set; }
        public string library { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sharing_status { get; set; }
        public string type { get; set; }
        public PlaylistAdditional additional { get; set; }

        public override string ToString() => id ?? base.ToString();
    }

    public struct PlaylistAdditional
    {
        public PlaylistSong[] songs { get; set; }
        public int songs_offset { get; set; }
        public int songs_total { get; set; }
        //public PlaylistSharingInfo sharing_info { get; set; }
    }

    //public struct PlaylistSharingInfo
    //{
    //	public string date_available { get; set; }
    //	public string date_expired { get; set; }
    //	public string id { get; set; }
    //	public string status { get; set; }
    //	public string url { get; set; }
    //}
    
    public struct PlaylistSong
    {
        public string id { get; set; }
        public string path { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public override string ToString() => title ?? base.ToString();
    }
}
