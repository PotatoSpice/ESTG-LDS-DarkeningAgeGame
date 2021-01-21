using System.Net.WebSockets;

namespace GameWebServer.Entities.Player
{
    public class ConnectionPlayer
    {
        public string playerId { get; private set; }
        public WebSocket socket { get; set; }

        public ConnectionPlayer(string p_playerId, WebSocket p_socket)
        {
            this.playerId = p_playerId;
            this.socket = p_socket;
        }
    }
}