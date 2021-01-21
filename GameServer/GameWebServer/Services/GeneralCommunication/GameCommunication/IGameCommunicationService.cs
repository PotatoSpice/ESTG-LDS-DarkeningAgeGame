using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameWebServer.Services
{
    [Obsolete("GameCommunicationService is deprecated, please use MessageHandlerService instead.")]
    public interface IGameCommunicationService : ICommunicationHandlerService
    {
        new Task ReceiveAsync(WebSocket socket, string receivedData);
    }   
}