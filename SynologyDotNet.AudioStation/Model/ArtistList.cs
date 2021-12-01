using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class ArtistList : ListResponseBase
    {
        [JsonProperty("artists")]
        public Artist[] Artists { get; set; }
    }
}
