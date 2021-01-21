using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Repositories;

namespace GameWebServer.Services
{
    [Obsolete("CommunicationHandlerService is deprecated, please use RoomHandlerService instead.")]
    public abstract class CommunicationHandlerService : ICommunicationHandlerService
    {
        protected IConnectionManager _connManager { get; set; }

        public CommunicationHandlerService(IConnectionManager connManager)
        {
            _connManager = connManager;
        }

        public virtual async Task OnConnected(ConnectionPlayer player)
        {
            _connManager.AddPlayer(player);
        }

        public virtual async Task OnDisconnected(string player)
        {
            await _connManager.RemovePlayer(player);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await OnDisconnected(_connManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.UTF8.GetBytes(message),
                                                                    offset: 0,
                                                                    count: Encoding.UTF8.GetBytes(message).Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string playerId, string message)
        {
            await SendMessageAsync(_connManager.GetPlayerById(playerId).socket, message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach(var conn in _connManager.GetAll())
            {
                if(conn.Value.socket.State == WebSocketState.Open)
                    await SendMessageAsync(conn.Value.socket, message);
            }
        }

        public abstract Task ReceiveAsync(WebSocket socket, string receivedData);
    }   
}