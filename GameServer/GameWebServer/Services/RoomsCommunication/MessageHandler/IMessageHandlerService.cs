using System.Net.WebSockets;
using System.Threading.Tasks;

namespace GameWebServer.Services
{
    public interface IMessageHandlerService : IRoomsHandlerService
    {
        new Task HandleLobbyAsync(string roomId, string playerId, WebSocket playerConn, string receivedData);

        new Task HandleGameAsync(string roomId, string playerId, WebSocket playerConn, string receivedData);

        Task CancelMatchmaking(string roomId);
    }   
}