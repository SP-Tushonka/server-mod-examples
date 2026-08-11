using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Servers.Ws;
using System.Net.WebSockets;
using System.Text;

namespace _24Websocket;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.sp-tarkov.examples.websocket";
    public string Name { get; init; } = "CustomWebSocketConnectionHandlerExample";
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

[Injectable(InjectionType = InjectionType.Singleton)]
public class CustomWebSocketConnectionHandler(
    ISptLogger<CustomWebSocketConnectionHandler> logger) : IWebSocketConnectionHandler
{
    public string GetHookUrl()
    {
        return "/custom/socket/";
    }

    public string GetSocketId()
    {
        return "My Custom WebSocket";
    }

    public Task OnConnectionAsync(WebSocket ws, HttpContext context, string sessionIdContext)
    {
        logger.Info("Custom web socket is now connected!");
        
        return Task.CompletedTask;
    }

    public async Task OnMessageAsync(byte[] rawData, WebSocketMessageType messageType, WebSocket ws, HttpContext context)
    {
        var msg = Encoding.UTF8.GetString(rawData);

        if (msg == "toodaloo")
        {
            await ws.SendAsync(Encoding.UTF8.GetBytes("toodaloo back!"), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public Task OnCloseAsync(WebSocket ws, HttpContext context, string sessionIdContext)
    {
        return Task.CompletedTask;
    }
}
