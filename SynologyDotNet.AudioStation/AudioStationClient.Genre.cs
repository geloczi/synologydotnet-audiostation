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
        public async Task<ApiListRessponse<GenreList>> ListGenresAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListRessponse<GenreList>>(SYNO_AudioStation_Genre, "list", limit, offset, 
                GetLibraryArg(),
                ("additional", "avg_rating"));
        }

        /// <summary>
        /// Searches the genres by name.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<GenreList>> SearchGenresByNameAsync(int limit, int offset, string keyword)
        {
            return await Client.QueryListAsync<ApiListRessponse<GenreList>>(SYNO_AudioStation_Genre, "list", limit, offset,
                GetLibraryArg(),
                ("additional", "avg_rating"),
                ("keyword", keyword),
                ("filter", keyword));
        }
    }
}
