using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Repositories;

namespace GameWebServer.Services
{
    [Obsolete("LobbyCommunicationService is deprecated, please use MessageHandlerService instead.")]
    public class LobbyCommunicationService : CommunicationHandlerService, ILobbyCommunicationService
    {
        public LobbyCommunicationService(IConnectionManager _connManager) : base(_connManager)
        {
        }
        
        // Add adicional logic for when a player connects
        public override async Task OnConnected(ConnectionPlayer player)
        {
            await base.OnConnected(player);

            await SendMessageToAllAsync($"{player.playerId} is now connected");
        }

        // Handle what happens when a message is received from the player client
        public override async Task ReceiveAsync(WebSocket socket, string receivedData)
        {
            var playerId = _connManager.GetId(socket);
            var message = $"{playerId} said: {receivedData}";

            await SendMessageToAllAsync(message);
        }
    }   
}