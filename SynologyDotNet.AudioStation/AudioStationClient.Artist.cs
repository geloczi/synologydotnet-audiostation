using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// List artists
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <returns></returns>
        public async Task<ApiListResponse<ArtistList>> ListArtistsAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListResponse<ArtistList>>(SYNO_AudioStation_Artist, "list", limit, offset, 
                GetLibraryArg(),
                ("additional", "avg_rating")).ConfigureAwait(false);
        }

        /// <summary>
        /// List artists
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <param name="genre">Filter by genre</param>
        /// <returns></returns>
        public async Task<ApiListResponse<ArtistList>> SearchArtistsByGenreAsync(int limit, int offset, string genre)
        {
            return await Client.QueryListAsync<ApiListResponse<ArtistList>>(SYNO_AudioStation_Artist, "list", limit, offset, 
                GetLibraryArg(),
                ("additional", "avg_rating"),
                ("genre", genre)).ConfigureAwait(false);
        }

        /// <summary>
        /// List artists
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <param name="keyword">Filter by keyword</param>
        /// <returns></returns>
        public async Task<ApiListResponse<ArtistList>> SearchArtistsByNameAsync(int limit, int offset, string keyword)
        {
            return await Client.QueryListAsync<ApiListResponse<ArtistList>>(SYNO_AudioStation_Artist, "list", limit, offset,
                GetLibraryArg(),
                ("additional", "avg_rating"),
                ("keyword", keyword),
                ("filter", keyword))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Download artist cover image
        /// </summary>
        /// <param name="artist">Artist name</param>
        /// <returns></returns>
        public async Task<ByteArrayData> GetArtistCoverAsync(string artist)
        {
            return await Client.QueryByteArrayAsync(SYNO_AudioStation_Cover, "getcover",
                GetLibraryArg(),
                ("artist_name", artist))
                .ConfigureAwait(false);
        }
    }
}
