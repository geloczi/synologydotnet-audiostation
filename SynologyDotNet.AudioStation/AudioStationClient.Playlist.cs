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
        /// Creates a new playlist.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="shared">True if the playlist is shared across all users, False if personal.</param>
        /// <returns>The ID of the created playlist.</returns>
        public async Task<ApiDataResponse<Id>> CreatePlaylistAsync(string name, bool shared)
        {
            return await Client.QueryObjectAsync<ApiDataResponse<Id>>(
                SYNO_AudioStation_Playlist, "create",
                ("name", name),
                ("library", shared ? Library.Shared : Library.Personal)
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a playlist by ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<ApiResponse> DeletePlaylistAsync(string id)
        {
            return await Client.QueryObjectAsync<ApiResponse>(
                SYNO_AudioStation_Playlist, "delete",
                ("id", id)
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists the playlists.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<PlaylistList>> ListPlaylistsAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListRessponse<PlaylistList>>(
                SYNO_AudioStation_Playlist, "list", limit, offset,
                GetLibraryArg()).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the songs on a specific playlist.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="additionalFields">Additional fields to load.</param>
        /// <returns></returns>
        public async Task<ApiDataResponse<Playlist>> GetPlaylistAsync(int limit, int offset, string id, SongQueryAdditional additionalFields = SongQueryAdditional.None)
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

            var test = await Client.QueryByteArrayAsync(SYNO_AudioStation_Playlist, "getinfo", args.ToArray()).ConfigureAwait(false);
            string json = Encoding.UTF8.GetString(test.Data);

            var playlists = await Client.QueryListAsync<ApiListRessponse<PlaylistList>>(
                SYNO_AudioStation_Playlist, "getinfo", limit, offset,
                args.ToArray()).ConfigureAwait(false);
            return new ApiDataResponse<Playlist>(playlists, playlists.Data?.Playlists?.FirstOrDefault() ?? default);
        }

        /// <summary>
        /// Adds the songs to the playlist.
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="songIds">The song IDs.</param>
        /// <returns></returns>
        public async Task<ApiResponse> AddSongsToPlaylist(string id, params string[] songIds)
        {
            return await Client.QueryObjectAsync<ApiResponse>(
                SYNO_AudioStation_Playlist, "updatesongs",
                ("id", id),
                ("offset", -1),
                ("limit", 0),
                ("songs", string.Join(",", songIds))
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes the selected song range from the specified playlist.
        /// The Playlist API does NOT support removing songs by ID directly, you must query the playlist first.
        /// </summary>
        /// <param name="id">Playlist ID.</param>
        /// <param name="startIndex">The index of the first song to be deleted.</param>
        /// <param name="count">The count of songs to be removed from start index.</param>
        public async Task<ApiResponse> RemoveSongsFromPlaylist(string id, int startIndex, int count)
        {
            return await Client.QueryObjectAsync<ApiResponse>(
                SYNO_AudioStation_Playlist, "updatesongs",
                ("id", id),
                ("offset", startIndex),
                ("limit", count),
                ("songs", string.Empty)
            ).ConfigureAwait(false);
        }
    }
}
