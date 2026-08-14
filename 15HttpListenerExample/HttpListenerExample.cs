using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Servers.Http;

namespace _15HttpListenerExample;

[Injectable(TypePriority = 0)]
public class HttpListenerExample : IHttpListener
{
    public bool CanHandle(HttpContext context)
    {
        return context.Request.Method == "GET" && context.Request.Path.Value!.Contains("/type-custom-url");
    }

    public async Task HandleAsync(MongoId sessionId, HttpContext context, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = 200;
        await context.Response.Body.WriteAsync("[1] This is the first example of a mod hooking into the HttpServer"u8.ToArray());
        await context.Response.StartAsync();
        await context.Response.CompleteAsync();
    }
}
