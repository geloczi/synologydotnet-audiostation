using System.Linq;
using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// Lists the playlists.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<PlaylistList>> ListPlaylistsAsync(int limit, int offset)
        {
            var result = await Client.QueryListAsync<ApiListRessponse<PlaylistList>>(SYNO_AudioStation_Playlist, "list", limit, offset,
                ("library", "all"));
            return result;
        }

        /// <summary>
        /// Gets the songs on a specific playlist.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<ApiDataResponse<Playlist>> GetPlaylistAsync(int limit, int offset, string id)
        {
            var result = await Client.QueryListAsync<ApiListRessponse<PlaylistList>>(SYNO_AudioStation_Playlist, "getinfo", limit, offset,
                ("library", "all"),
                ("id", id),
                ("additional", "songs") //("additional", "songs_song_tag,songs_song_audio,songs_song_rating,sharing_info")
            );
            return new ApiDataResponse<Playlist>(result, result.Data?.playlists?.FirstOrDefault() ?? default);
        }
    }
}
