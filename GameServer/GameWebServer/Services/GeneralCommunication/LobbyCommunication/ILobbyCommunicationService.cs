using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameWebServer.Services
{
    [Obsolete("LobbyCommunicationService is deprecated, please use MessageHandlerService instead.")]
    public interface ILobbyCommunicationService : ICommunicationHandlerService
    {
        new Task ReceiveAsync(WebSocket socket, string receivedData);
    }   
}