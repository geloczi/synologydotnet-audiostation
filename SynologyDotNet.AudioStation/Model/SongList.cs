using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class SongList : ListResponseBase
    {
        [JsonProperty("songs")]
        public Song[] Songs { get; set; }
    }
}
