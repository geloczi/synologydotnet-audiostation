using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class AlbumList : ListResponseBase
    {
        [JsonProperty("albums")]
        public Album[] Albums { get; set; }
    }
}
