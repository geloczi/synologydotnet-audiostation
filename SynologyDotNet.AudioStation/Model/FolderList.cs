using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class FolderList : ListResponseBase
    {
        [JsonProperty("items")]
        public Folder[] Items { get; set; }
    }
}
