using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Helpers;
using SynologyDotNet.Core.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// List songs
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <param name="additionalFields">Additional fields to load</param>
        /// <param name="queryParameters">Filter parameters</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<SongList>> ListSongsAsync(int limit, int offset, SongQueryAdditional additionalFields, params (SongQueryParameter, object)[] queryParameters)
        {
            var args = new List<(string, object)>(queryParameters.Select(f => (f.Item1.ToString(), f.Item2)));
            args.Add(GetLibraryArg());
            if (additionalFields != SongQueryAdditional.None)
            {
                args.Add(("additional", string.Join(",", (new[] {
                        SongQueryAdditional.song_audio,
                        SongQueryAdditional.song_rating,
                        SongQueryAdditional.song_tag
                    })
                    .Where(x => additionalFields.HasFlag(x))
                    .Select(x => x.ToString()))));
            }
            return await Client.QueryListAsync<ApiListRessponse<SongList>>(SYNO_AudioStation_Song, "list", limit, offset, args.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// List songs
        /// </summary>
        /// <param name="limit">Maximum number of items to return</param>
        /// <param name="offset">Start position in the list (use it for paging)</param>
        /// <param name="title">Title to search</param>
        /// <param name="additionalFields">Additional fields to load</param>
        /// <returns></returns>
        public async Task<ApiListRessponse<SongList>> SearchSongsByTitleAsync(int limit, int offset, string title, SongQueryAdditional additionalFields)
        {
            var args = new List<(string, object)>();
            args.Add(GetLibraryArg());
            args.Add(("title", title));
            args.Add(("keyword", title));
            if (additionalFields != SongQueryAdditional.None)
            {
                args.Add(("additional", string.Join(",", (new[] {
                        SongQueryAdditional.song_audio,
                        SongQueryAdditional.song_rating,
                        SongQueryAdditional.song_tag
                    })
                    .Where(x => additionalFields.HasFlag(x))
                    .Select(x => x.ToString()))));
            }
            return await Client.QueryListAsync<ApiListRessponse<SongList>>(SYNO_AudioStation_Song, "search", limit, offset, args.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a song by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiListRessponse<SongList>> GetSongByIdAsync(string id)
        {
            var args = new List<(string, object)>();
            args.Add(("id", id));
            args.Add(("additional", "song_tag, song_audio, song_rating")); // request detailed song info
            return await Client.QueryObjectAsync<ApiListRessponse<SongList>>(SYNO_AudioStation_Song, "getinfo", args.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Set song rating
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="rating">Value from 0 to 5</param>
        /// <returns></returns>
        public async Task<ApiResponse> RateSongAsync(string songId, int rating)
        {
            if (rating < 0 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Value range: 0 - 5");
            return await Client.QueryObjectAsync<ApiResponse>(SYNO_AudioStation_Song, "setrating",
                ("id", songId),
                ("rating", rating)).ConfigureAwait(false);
        }

        /// <summary>
        /// Download the song from the server.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="transcode">Transcode method</param>
        /// <param name="songId">ID of the song to download</param>
        /// <param name="positionSeconds">Start position in seconds</param>
        /// <param name="readStreamAction">The action called to download the song bytes.</param>
        /// <returns></returns>
        public async Task StreamSongAsync(
            CancellationToken cancellationToken,
            TranscodeMode transcode,
            string songId,
            double positionSeconds,
            Action<StreamResult> readStreamAction)
        {
            var req = CreateSongStreamRequest(SYNO_AudioStation_Stream, transcode, songId, positionSeconds);
            await Client.QueryStreamAsync(req, readStreamAction, cancellationToken).ConfigureAwait(false);
        }

        private RequestBuilder CreateSongStreamRequest(string apiName, TranscodeMode transcode, string songId, double positionInSeconds)
        {
            string method = "stream";
            string subEndpoint = null;
            string format = null;
            switch (transcode)
            {
                case TranscodeMode.MP3_128Kbps:
                case TranscodeMode.MP3_192Kbps:
                case TranscodeMode.MP3_256Kbps:
                case TranscodeMode.MP3_320Kbps:
                    subEndpoint = "/0.mp3";
                    method = "transcode";
                    format = "mp3";
                    break;
                case TranscodeMode.WAV:
                    subEndpoint = "/0.wav";
                    method = "transcode";
                    format = "wav";
                    break;
            }
            var req = new RequestBuilder(Client.GetApiInfo(apiName), subEndpoint).Method(method).SetParam("id", songId);
            if (!string.IsNullOrEmpty(format))
                req.SetParam("format", format);
            if (positionInSeconds > 0)
                req.SetParam("position", Math.Round(positionInSeconds, 4).ToString(CultureInfo.InvariantCulture));
            switch (transcode)
            {
                case TranscodeMode.MP3_128Kbps:
                    req.SetParam("bitrate", "128000");
                    break;
                case TranscodeMode.MP3_192Kbps:
                    req.SetParam("bitrate", "192000");
                    break;
                case TranscodeMode.MP3_256Kbps:
                    req.SetParam("bitrate", "256000");
                    break;
                case TranscodeMode.MP3_320Kbps:
                    req.SetParam("bitrate", "320000");
                    break;
            }
            return req;
        }
    }
}
