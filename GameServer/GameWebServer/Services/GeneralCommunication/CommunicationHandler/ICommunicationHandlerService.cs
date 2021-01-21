using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;

namespace GameWebServer.Services
{
    [Obsolete("CommunicationHandlerService is deprecated, please use RoomHandlerService instead.")]
    public interface ICommunicationHandlerService
    {
        Task OnConnected(ConnectionPlayer player);

        Task OnDisconnected(string player);

        Task OnDisconnected(WebSocket socket);

        Task SendMessageAsync(WebSocket socket, string message);

        Task SendMessageAsync(string socketId, string message);

        Task SendMessageToAllAsync(string message);

        Task ReceiveAsync(WebSocket socket, string receivedData);
    }   
}