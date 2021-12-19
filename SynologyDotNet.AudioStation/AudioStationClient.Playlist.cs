using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                GetLibraryArg());
            return result;
        }

        /// <summary>
        /// Gets the songs on a specific playlist.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="additionalFields">Additional fields to load.</param>
        /// <returns></returns>
        public async Task<ApiDataResponse<Playlist>> GetPlaylistAsync(int limit, int offset, string id, SongQueryAdditional additionalFields)
        {
            var args = new List<(string, object)>();
            args.Add(GetLibraryArg());
            args.Add(("id", id));

            var additionalFieldNames = new List<string>();
            if (additionalFields != SongQueryAdditional.None)
            {
                additionalFieldNames.AddRange((new[] {
                        SongQueryAdditional.song_audio,
                        SongQueryAdditional.song_rating,
                        SongQueryAdditional.song_tag
                    })
                    .Where(x => additionalFields.HasFlag(x))
                    .Select(x => "songs_" + x.ToString()));
            }
            else
            {
                additionalFieldNames.Add("songs");
            }
            args.Add(("additional", string.Join(",", additionalFieldNames)));

            var test = await Client.QueryByteArrayAsync(SYNO_AudioStation_Playlist, "getinfo", args.ToArray());
            string json = Encoding.UTF8.GetString(test.Data);

            var playlists = await Client.QueryListAsync<ApiListRessponse<PlaylistList>>(SYNO_AudioStation_Playlist, "getinfo", limit, offset, args.ToArray());
            return new ApiDataResponse<Playlist>(playlists, playlists.Data?.Playlists?.FirstOrDefault() ?? default);
        }
    }
}
