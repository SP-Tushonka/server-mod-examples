using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Servers.Http;

namespace _15HttpListenerExample;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.httplistener";
    public string Name { get; init; } = "HttpListenerExample";
    public string Author { get; init; } = "SPTarkov";
    public List<string>? Contributors { get; init; }
    public SemanticVersioning.Version Version { get; init; } = new("1.0.0");
    public SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    
    
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
    public bool HasPrepatcher { get; init; } = false;
}

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
