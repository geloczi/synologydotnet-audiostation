using System.Threading.Tasks;
using SynologyDotNet.Core.Responses;

namespace SynologyDotNet.AudioStation
{
    public partial class AudioStationClient
    {
        public async Task<ApiListRessponse<ComposerList>> ListComposersAsync(int limit, int offset)
        {
            return await Client.QueryListAsync<ApiListRessponse<ComposerList>>(SYNO_AudioStation_Composer, "list", limit, offset, GetLibraryArg());
        }
    }
}
