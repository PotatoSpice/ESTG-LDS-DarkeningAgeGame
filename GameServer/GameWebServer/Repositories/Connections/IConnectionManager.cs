using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;

namespace GameWebServer.Repositories
{
    [Obsolete("ConnectionManager is deprecated, please use RoomManager instead.")]
    public interface IConnectionManager
    {
        ConnectionPlayer GetPlayerById(string id);

        ConcurrentDictionary<string, ConnectionPlayer> GetAll();

        string GetId(WebSocket socket);

        void AddPlayer(ConnectionPlayer player);

        Task RemovePlayer(string id);
    }
}