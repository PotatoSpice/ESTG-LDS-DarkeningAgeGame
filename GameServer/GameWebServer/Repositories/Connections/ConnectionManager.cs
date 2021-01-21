using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;

namespace GameWebServer.Repositories
{
    [Obsolete("ConnectionManager is deprecated, please use RoomManager instead.")]
    public class ConnectionManager : IConnectionManager
    {
        private ConcurrentDictionary<string, ConnectionPlayer> _players = new ConcurrentDictionary<string, ConnectionPlayer>();

        public ConnectionPlayer GetPlayerById(string id)
        {
            return _players.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, ConnectionPlayer> GetAll()
        {
            return _players;
        }

        public string GetId(WebSocket socket)
        {
            return _players.FirstOrDefault(p => p.Value.socket == socket).Key;
        }
        
        public void AddPlayer(ConnectionPlayer player)
        {
            _players.TryAdd(player.playerId, player);
        }

        // Removes a player from the collection and terminates it's socket connection
        public async Task RemovePlayer(string id)
        {
            ConnectionPlayer player;
            _players.TryRemove(id, out player);

            await player.socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Connection closed by manager ...",
                                    cancellationToken: CancellationToken.None);
        }
    }   
}