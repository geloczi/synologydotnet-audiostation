using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// List albums
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <param name="artist">Filter by artist name</param>
        /// <param name="queryParameters">Filter parameters</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<AlbumList>> ListAlbumsAsync(int limit, int offset, string artist = null, params (AlbumQueryParameter, object)[] queryParameters)
        {
            var args = new List<(string, object)>(queryParameters.Select(f => (f.Item1.ToString(), f.Item2)));
            args.Add(GetLibraryArg());
            args.Add(("additional", "avg_rating"));
            if (!string.IsNullOrWhiteSpace(artist))
                args.Add(("artist", artist));
            
            return await Client.QueryListAsync<ApiListRessponse<AlbumList>>(SYNO_AudioStation_Album, "list", limit, offset, args.ToArray());
        }

        /// <summary>
        /// Download album cover image
        /// </summary>
        /// <param name="artist">Artist name</param>
        /// <param name="album">Album title</param>
        /// <returns></returns>
        public async Task<ByteArrayData> GetAlbumCoverAsync(string artist, string album)
        {
            return await Client.QueryByteArrayAsync(SYNO_AudioStation_Cover, "getcover",
                ("album_name", album),
                ("album_artist_name", artist));
        }

        public async Task<ApiListRessponse<AlbumList>> SearchAlbumsByNameAsync(int limit, int offset, string keyword)
        {
            return await Client.QueryListAsync<ApiListRessponse<AlbumList>>(SYNO_AudioStation_Album, "list", limit, offset,
                GetLibraryArg(),
                ("additional", "avg_rating"),
                ("keyword", keyword),
                ("filter", keyword));
        }
    }
}
