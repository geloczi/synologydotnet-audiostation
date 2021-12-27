using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Helpers;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        private const string UsermanEndpoint = "webman/3rdparty/AudioStation/webUI/audio_userman.cgi";

        /// <summary>
        /// Starts indexing service to update audio library.
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResponse> StartReIndex()
        {
            var req = new RequestBuilder().SetEndpoint(UsermanEndpoint);
            req["action"] = "do_reindex";
            return await Client.QueryObjectAsync<ApiResponse>(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the state of the indexing service.
        /// </summary>
        /// <returns></returns>
        public async Task<ReIndexStateResponse> GetReIndexState()
        {
            var req = new RequestBuilder().SetEndpoint(UsermanEndpoint);
            req["action"] = "load_reindex";
            return await Client.QueryObjectAsync<ReIndexStateResponse>(req).ConfigureAwait(false);
        }
    }
}
