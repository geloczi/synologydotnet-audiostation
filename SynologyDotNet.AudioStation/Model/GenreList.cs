using Newtonsoft.Json;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation.Model
{
    public class GenreList : ListResponseBase
    {
        [JsonProperty("genres")]
        public Genre[] Genres { get; set; }
    }
}
