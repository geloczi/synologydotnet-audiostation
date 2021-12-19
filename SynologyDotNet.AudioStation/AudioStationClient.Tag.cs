using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Helpers;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        /// <summary>
        /// This exists only if the AudioStation package has been installed.
        /// This endpoint is used to edit song metadata prgorammatically.
        /// </summary>
        private const string TagEditorEndpoint = "webman/3rdparty/AudioStation/tagEditorUI/tag_editor.cgi";

        /// <summary>
        /// Query song tags
        /// </summary>
        /// <param name="paths">The internal path of the music file. Must contain forward slaashes '/'</param>
        /// <returns></returns>
        public async Task<FileTags> GetSongFileTags(params string[] paths)
        {
            if (paths?.Any() != true)
                throw new ArgumentNullException(nameof(paths));
            if (paths.Any(p => p.Contains("\\")))
                throw new ArgumentException("Invalid path. Path must contain forward slashes '/', not back-slashes '\\'");
            var req = new RequestBuilder().SetEndpoint(TagEditorEndpoint).Action("load");
            req["audioInfos"] = JsonConvert.SerializeObject(paths.Select(p => new { path = p }));
            req["requestFrom"] = string.Empty;
            var result = await Client.QueryObjectAsync<FileTags>(req);
            return result;
        }

        /// <summary>
        /// Batch edit music file tags
        /// </summary>
        /// <param name="change"></param>
        /// <returns></returns>
        public async Task<ApiResponse> SetSongFileTags(FileTagChange change)
        {
            if (change?.AudioInfos?.Any() != true)
                throw new ArgumentNullException($"{nameof(change)}.{nameof(change.AudioInfos)}");

            var req = new RequestBuilder().SetEndpoint(TagEditorEndpoint).Action("apply");
            req["data"] = JsonConvert.SerializeObject(new object[] { change });
            var result = await Client.QueryObjectAsync<ApiResponse>(req);
            return result;
        }

    }
}
