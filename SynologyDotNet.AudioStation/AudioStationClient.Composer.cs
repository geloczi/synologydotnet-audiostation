using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// Lists the composers.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<ComposerList>> ListComposersAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListRessponse<ComposerList>>(SYNO_AudioStation_Composer, "list", limit, offset, 
                GetLibraryArg(), 
                ("additional", "avg_rating"));
        }

        /// <summary>
        /// Searches the composers by name.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<ComposerList>> SearchComposersByNameAsync(int limit, int offset, string keyword)
        {
            return await Client.QueryListAsync<ApiListRessponse<ComposerList>>(SYNO_AudioStation_Composer, "list", limit, offset,
                GetLibraryArg(),
                ("additional", "avg_rating"),
                ("keyword", keyword),
                ("filter", keyword));
        }
    }
}
