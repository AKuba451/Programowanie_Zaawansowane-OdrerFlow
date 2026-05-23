using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OrderFlow.Tests;

public class TestHttpMessageHandler : HttpMessageHandler
{
    public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;
    public string ResponseContent { get; set; } = "";
    public HttpRequestMessage? LastRequest { get; private set; }
    public int RequestCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        RequestCount++;

        var response = new HttpResponseMessage(ResponseStatusCode)
        {
            Content = new StringContent(ResponseContent)
        };

        return Task.FromResult(response);
    }
}