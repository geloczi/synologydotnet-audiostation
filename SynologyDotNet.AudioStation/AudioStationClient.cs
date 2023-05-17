using System.Collections.Generic;
using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    /// <summary>
    /// Connects to AudioStation APIs
    /// </summary>
    public partial class AudioStationClient : StationConnectorBase
    {
        #region Apis

        const string SYNO_AudioStation_Info = "SYNO.AudioStation.Info";
        const string SYNO_AudioStation_Album = "SYNO.AudioStation.Album";
        const string SYNO_AudioStation_Composer = "SYNO.AudioStation.Composer";
        const string SYNO_AudioStation_Genre = "SYNO.AudioStation.Genre";
        const string SYNO_AudioStation_Artist = "SYNO.AudioStation.Artist";
        const string SYNO_AudioStation_Folder = "SYNO.AudioStation.Folder";
        const string SYNO_AudioStation_Song = "SYNO.AudioStation.Song";
        const string SYNO_AudioStation_Cover = "SYNO.AudioStation.Cover";
        const string SYNO_AudioStation_Stream = "SYNO.AudioStation.Stream";
        const string SYNO_AudioStation_Search = "SYNO.AudioStation.Search";
        const string SYNO_AudioStation_Lyrics = "SYNO.AudioStation.Lyrics";
        const string SYNO_AudioStation_Playlist = "SYNO.AudioStation.Playlist";

        protected override string[] GetImplementedApiNames() => new string[]
        {
            SYNO_AudioStation_Info,
            SYNO_AudioStation_Album,
            SYNO_AudioStation_Composer,
            SYNO_AudioStation_Genre,
            SYNO_AudioStation_Artist,
            SYNO_AudioStation_Folder,
            SYNO_AudioStation_Song,
            SYNO_AudioStation_Cover,
            SYNO_AudioStation_Stream,
            SYNO_AudioStation_Search,
            SYNO_AudioStation_Lyrics,
            SYNO_AudioStation_Playlist
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether only the personal music folder is used.
        /// True if only the current user's personal music folder is used. 
        /// False if the personal and the shared music are used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if only the current user's personal music folder is used; <c>false</c> if both the personal and the shared music are used.
        /// </value>
        public bool PersonalMusicOnly { get; set; }

        #endregion Properties
        
        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="AudioStationClient" /> class.</summary>
        public AudioStationClient() : base()
        {
        }

        #endregion Constructor

        #region Public Methods        

        /// <summary>
        /// Searches the music library.
        /// </summary>
        /// <param name="keyword">The text to search.</param>
        /// <returns></returns>
        public async Task<ApiDataResponse<SearchResults>> SearchAsync(string keyword)
        {
            var args = new List<(string, object)>();
            args.Add(GetLibraryArg());
            args.Add(("additional", "song_tag,song_audio,song_rating"));
            args.Add(("keyword", keyword));
            return await Client.QueryObjectAsync<ApiDataResponse<SearchResults>>(SYNO_AudioStation_Search, "list", args.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// List folders
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <param name="folderId">Root folder ID, the children directories will be listed in the response. Not recursive.</param>
        /// <returns></returns>
        public async Task<ApiListResponse<FolderList>> ListFoldersAsync(int limit, int offset, string folderId = null)
        {
            var args = new List<(string, object)>();
            args.Add(GetLibraryArg());
            if (!string.IsNullOrEmpty(folderId))
                args.Add(("id", folderId));

            return await Client.QueryListAsync<ApiListResponse<FolderList>>(SYNO_AudioStation_Folder, "list", limit, offset, args.ToArray()).ConfigureAwait(false);
        }
        
        // Use tageditor instead!
        //#region Lyrics
        //public async Task<ApiDataResponse<Lyrics>> GetLyricsAsync(string songId)
        //{
        //	var result = await Client.QueryAsync<ApiDataResponse<Lyrics>>(Syno_AudioStation_Lyrics, "getlyrics", ("id", songId));
        //	return result;
        //}

        //public async Task<ApiResponse> SetLyricsAsync(string songId, string lyrics)
        //{
        //	var result = await Client.QueryAsync<ApiResponse>(Syno_AudioStation_Lyrics, "setlyrics",
        //		("id", songId),
        //		("lyrics", lyrics));
        //	return result;
        //}
        //#endregion

        #endregion

        #region Private Methods

        private (string, string) GetLibraryArg() => ("library", PersonalMusicOnly ? Library.Personal : Library.All);

        #endregion

        #region Classes

        private static class Library
        {
            public const string All = "all";

            public const string Personal = "personal";

            public const string Shared = "shared";
        }

        #endregion
    }
}