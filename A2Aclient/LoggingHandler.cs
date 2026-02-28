// helper class for outbound logging

namespace A2Aclient;
public class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler inner) : base(inner) { }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Console.WriteLine($"[http] {request.Method} {request.RequestUri}");
        var response = await base.SendAsync(request, ct);
        Console.WriteLine($"[http] response {((int)response.StatusCode)} {response.RequestMessage!.RequestUri}");
        return response;
    }
}