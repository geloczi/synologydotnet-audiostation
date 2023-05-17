using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// Lists the genres.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public async Task<ApiListResponse<GenreList>> ListGenresAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListResponse<GenreList>>(SYNO_AudioStation_Genre, "list", limit, offset, 
                GetLibraryArg(),
                ("additional", "avg_rating"))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Searches the genres by name.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public async Task<ApiListResponse<GenreList>> SearchGenresByNameAsync(int limit, int offset, string keyword)
        {
            return await Client.QueryListAsync<ApiListResponse<GenreList>>(SYNO_AudioStation_Genre, "list", limit, offset,
                GetLibraryArg(),
                ("additional", "avg_rating"),
                ("keyword", keyword),
                ("filter", keyword))
                .ConfigureAwait(false);
        }
    }
}
