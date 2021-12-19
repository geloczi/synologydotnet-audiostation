using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class ComposerList : ListResponseBase
    {
        [JsonProperty("composers")]
        public Composer[] Composers { get; set; }
    }
}
