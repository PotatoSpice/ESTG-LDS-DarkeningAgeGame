using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using GameWebServer.Entities.Player;

namespace GameWebServer.Entities
{
    public class PlayersManager
    {
        private IDictionary<string, RoomPlayer> _players;
        public string hostID { get; private set; }
        public bool matchmakingRoom { get; private set; }
        public int PlayerCount { get { return _players.Count; } }
        public ICollection<RoomPlayer> PlayerList { get { return _players.Values; } }
        
        public PlayersManager(RoomPlayer p_host, bool findMatch)
        {
            this._players = new Dictionary<string, RoomPlayer>(4);
            this._players.Add(p_host.playerId, p_host);
            this.hostID = p_host.playerId;
            this.matchmakingRoom = findMatch;
        }

        public PlayersManager(ICollection<RoomPlayer> p_players)
        {
            this._players = new Dictionary<string, RoomPlayer>();
            foreach(RoomPlayer player in p_players)
            {
                this._players[player.playerId] = player;
            }
            this.hostID = _players.Keys.First<string>();
        }

        public bool ContainsPlayer(string playerId)
        {
            return _players.ContainsKey(playerId);
        }

        public bool ContainsPlayerConn(WebSocket conn)
        {
            foreach(RoomPlayer player in _players.Values)
            {
                if (player.socket == conn) return true;
            }
            return false;
        }

        public RoomPlayer GetPlayer(string playerId)
        {
            RoomPlayer player = null;
            _players.TryGetValue(playerId, out player);
            return player;
        }

        // Adds a player into the room, maximum 4 players
        public bool AddPlayer(RoomPlayer player)
        {
            if (matchmakingRoom == false && _players.Count >= 4
                || matchmakingRoom == true && _players.Count >= 2) return false;
            _players[player.playerId] = player;
            return true;
        }

        // Updates an existing player's data with the new one from parameter
        public bool UpdatePlayer(RoomPlayer player)
        {
            if (!_players.ContainsKey(player.playerId)) return false;
            _players[player.playerId] = player;
            return true;
        }

        // Removes a player from the room. The removed player reference is returned.
        // If the players was the host and the room still has players, a new host is elected.
        public bool RemovePlayer(string playerId, out RoomPlayer removed)
        {
            if (_players.Remove(playerId, out removed))
            {
                if (_players.Count != 0) hostID = _players.Keys.ToList().First<string>();
                else hostID = null;
                return true;
            }
            return false;
        }

        // Removes a player from the room. If the players was the host, a new host is elected.
        public bool RemovePlayer(string playerId)
        {
            RoomPlayer dummy;
            return RemovePlayer(playerId, out dummy);
        }

        // Sets a new host player for the room, if the player exists
        public bool UpdateHost(string hostId)
        {
            if (_players.ContainsKey(hostId))
            {
                this.hostID = hostID;
                return true;
            }
            return false;
        }
    }
}