using System.Text;
using System.Threading.Tasks;
using SynologyDotNet.AudioStation.Model;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        public async Task<ApiListRessponse<GenreList>> ListGenresAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListRessponse<GenreList>>(SYNO_AudioStation_Genre, "list", limit, offset, 
                GetLibraryArg(),
                ("additional", "avg_rating"));
        }
    }
}
