using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Site.Shared.Api.Server;

public class ServerLoginMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($">> {request.RequestUri}");
            var response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine($"<< {response.StatusCode}");

            return response;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Failed: {ex.StatusCode}");
            throw;
        }
    }
}