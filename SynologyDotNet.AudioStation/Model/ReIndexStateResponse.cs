using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class ReIndexStateResponse : ApiResponse
    {
        [JsonProperty("reindexing")]
        public bool ReIndexing { get; set; }
    }
}
