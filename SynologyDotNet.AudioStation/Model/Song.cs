using System;
using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    [Flags]
    public enum SongQueryAdditional : short
    {
        None = 0,
        song_tag = 1,
        song_audio = 2,
        song_rating = 4,
        All = ~None
    }
    
    public enum SongQueryParameter
    {
        id,
        artist,
        album_artist,
        album,
        sort_by,
        sort_direction
    }
    
    public static class SongSortBy
    {
        public const string title = "title";
        public const string song_rating = "song_rating";
        public const string album = "album";
        public const string album_artist = "album_artist";
        public const string year = "year";
        public const string track = "track";
        public const string random = "random";
    }

    public class Song
    {
        /// <summary>
        /// Gets or sets the identifier of the song.
        /// Songs in the personal library have "music_p_" prefix.
        /// Songs in the shared library have "music_" prefix.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("additional")]
        public SongAdditional Additional { get; set; }

        public override string ToString()
        {
            return $"{ID}, {Path}";
        }
    }

    public struct SongAdditional
    {
        [JsonProperty("song_audio")]
        public SongAudio Audio { get; set; }
        [JsonProperty("song_rating")]
        public SongRating Rating { get; set; }
        [JsonProperty("song_tag")]
        public SongTag Tag { get; set; }
    }

    public struct SongAudio
    {
        [JsonProperty("bitrate")]
        public int Bitrate { get; set; }
        [JsonProperty("channel")]
        public int Channels { get; set; }
        [JsonProperty("codec")]
        public string Codec { get; set; }
        [JsonProperty("container")]
        public string Container { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("filesize")]
        public int FileSize { get; set; }
        [JsonProperty("frequency")]
        public int Frequency { get; set; }
    }

    public struct SongRating
    {
        [JsonProperty("rating")]
        public int Value { get; set; }
    }

    public struct SongTag
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
        [JsonProperty("rg_album_gain")]
        public string AlbumGain { get; set; }
        [JsonProperty("rg_album_peak")]
        public string AlbumPeak { get; set; }
        [JsonProperty("rg_track_gain")]
        public string TrackGain { get; set; }
        [JsonProperty("rg_track_peak")]
        public string TrackPeak { get; set; }
        [JsonProperty("track")]
        public int Track { get; set; }
        [JsonProperty("year")]
        public int Year { get; set; }
    }
}
