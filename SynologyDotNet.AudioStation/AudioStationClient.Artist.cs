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
        public async Task<ApiListRessponse<ArtistList>> ListArtistsAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListRessponse<ArtistList>>(SYNO_AudioStation_Artist, "list", limit, offset, GetLibraryArg()); //personal
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
                ("artist_name", artist));
        }
    }
}
