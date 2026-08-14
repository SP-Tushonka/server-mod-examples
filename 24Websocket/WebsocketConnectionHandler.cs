using SPTarkov.DI.Annotations;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Servers.Ws;
using System.Net.WebSockets;
using System.Text;

namespace _24Websocket;

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
