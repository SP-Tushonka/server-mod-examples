using System.Net.WebSockets;
using SPTarkov.Common.Models.Logging;
using SPTarkov.Server.Core.Servers.Ws.Message;

namespace _24Websocket;

public class WebsocketMessageHandler(
    ISptLogger<WebsocketMessageHandler> logger) : ISptWebSocketMessageHandler
{
    public Task OnSptMessageAsync(string sessionID, WebSocket client, byte[] rawData)
    {
        logger.Info($"Custom SPT WebSocket Message handler received a message for {sessionID}: {rawData.ToString()}");
        return Task.CompletedTask;
    }
}
