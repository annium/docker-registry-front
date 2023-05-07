using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Site.Shared.Api.Server;

public class ServerLoginMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // TODO: implement auth at server
        return await base.SendAsync(request, cancellationToken);
    }
}