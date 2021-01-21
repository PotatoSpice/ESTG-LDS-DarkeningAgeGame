using System.Net.WebSockets;

namespace GameWebServer.Entities.Player
{
    public class RoomPlayer : ConnectionPlayer
    {

        public string faction { get; set; }
        public int placement { get; set; }
        public int armiesCreated { get; set; }
        public int regionsConquered { get; set; }
        public string getFaction()
        {
            return faction; 
        }

        public RoomPlayer(string p_playerId, WebSocket p_socket, string p_faction) : base(p_playerId, p_socket)
        {
            this.faction = p_faction;
            placement = 0;
            armiesCreated = 0;
            regionsConquered = 0;
        }

        public RoomPlayer(string p_playerId, WebSocket p_socket) : base(p_playerId, p_socket)
        {
            // Default
            placement = 0;
            armiesCreated = 0;
            regionsConquered = 0;
        }
    }
}