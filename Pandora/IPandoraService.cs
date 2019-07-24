using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pandora
{
    public interface IPandoraService
    {
        Task<PandoraState> ConnectAsync(RESTfulService service);
        Task<bool> LoginAsync(PandoraConfig config, RESTfulService service, PandoraState pandoraState);
        Task<HttpResponseMessage> DoStandardCallAsync(RESTfulService service, string method, Dictionary<string, object> data, bool useSsl, PandoraState pandoraState, CryptoConfig config);
        Task<List<PandoraStation>> GetPandoraStationsAsync(PandoraConfig config, RESTfulService service, PandoraState pandoraState);
        Task<bool> RatePandoraSongAsync(RESTfulService service, PandoraState pandoraState, PandoraStation station, PandoraSong song, bool rating);
        Task<List<PandoraSong>> GetPandoraPlaylistAsync(RESTfulService service, PandoraState pandoraState, PandoraStation station);
    }
}
