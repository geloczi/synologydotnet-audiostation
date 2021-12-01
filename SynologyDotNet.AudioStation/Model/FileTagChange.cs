using System.Collections.Generic;
using Newtonsoft.Json;

namespace SynologyDotNet.AudioStation.Model
{
    public class FileTagChange : FileTag
    {
        public FileTagChange() { }

        public FileTagChange(FileTag ft)
        {
            Album = ft.Album;
            AlbumArtist = ft.AlbumArtist;
            Artist = ft.Artist;
            Comment = ft.Comment;
            Composer = ft.Composer;
            Disc = ft.Disc;
            Genre = ft.Genre;
            Title = ft.Title;
            Track = ft.Track;
            Year = ft.Year;
        }

        /// <summary>
        /// Changes will be applied on these files
        /// </summary>
        [JsonProperty("audioInfos")]
        public List<FileTag> AudioInfos { get; set; } = new List<FileTag>();

        [JsonProperty("lyrics")]
        public string Lyrics { get; set; } = string.Empty;
        [JsonProperty("coverType")]
        public string CoverType { get; set; } = "original_image";
        [JsonProperty("coverPath")]
        public string CoverPath { get; set; } = string.Empty;
        [JsonProperty("codePage")]
        public string CodePage { get; set; } = "SYNO_NO_CODE_PAGE_CONVERT";
    }
}
