using Newtonsoft.Json;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public class ComposerList : ListResponseBase
    {
        [JsonProperty("composers")]
        public Composer[] Composers { get; set; }
    }
}
