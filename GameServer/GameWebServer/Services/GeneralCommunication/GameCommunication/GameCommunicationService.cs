using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Repositories;

namespace GameWebServer.Services
{
    [Obsolete("GameCommunicationService is deprecated, please use MessageHandlerService instead.")]
    public class GameCommunicationService : CommunicationHandlerService, IGameCommunicationService
    {

        public GameCommunicationService(IConnectionManager _connManager) : base(_connManager)
        {
        }

        // Add adicional logic for when a player connects
        public override async Task OnConnected(ConnectionPlayer player)
        {
            await base.OnConnected(player);

            await SendMessageToAllAsync($"{player.playerId} has connected");
        }

        // Handle what happens when a message is received from the player client
        public override async Task ReceiveAsync(WebSocket socket, string receivedData)
        {
            var playerId = _connManager.GetId(socket);
            var message = $"{playerId} said: {receivedData}";

            await SendMessageToAllAsync(message);
            // var message = Protocol.executeAction( receivedData, (MapManager) _mapManager);
            // Console.WriteLine(message); 
            // await SendMessageToAllAsync(message);
        }
    }   
}