using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;

namespace GameWebServer.Services
{
    public interface IRoomsHandlerService
    {
        Task<string> OnConnectedHostLobby(RoomPlayer host_player, bool findMatch);

        Task<ICollection<string>> OnConnectedJoinLobby(string roomId, RoomPlayer player);

        Task OnPlayerLeft(string roomId, string player);

        string CheckIfPlayerInGame(string playerId);
        
        bool CheckIfPlayerInStartedGame(string roomId, string playerId);

        Task<ICollection<RoomPlayer>> UpdateConnection(string roomId, string playerId, WebSocket newConn);

        Task SendMessageAsync(WebSocket socket, string sendData);

        Task SendToOthersInLobbyAsync(string roomId, string playerToIgnore, string sendData);

        Task SendToAllInLobbyAsync(string roomId, string sendData);

        Task SendToAllInGameAsync(string roomId, string sendData);

        Task HandleLobbyAsync(string roomId, string playerId, WebSocket playerConn, string receivedData);

        Task HandleGameAsync(string roomId, string playerId, WebSocket playerConn, string receivedData);
    }   
}