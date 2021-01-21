using System.Collections.Generic;

namespace GameWebServer.Models.Responses
{
    public class RoomPlayerDTO
    {
        public string playerId { get; set; }
        public string faction { get; set; }

        public RoomPlayerDTO(string p_playerId, string p_faction)
        {
            this.playerId = p_playerId;
            this.faction = p_faction;
        }
    }

    public class GameUpdatedConnResponse
    {
        public string eventType { get; private set; }
        public ICollection<RoomPlayerDTO> data { get; set; }  

        public GameUpdatedConnResponse(ICollection<RoomPlayerDTO> p_data)
        {
            this.eventType = "GameUpdatedConn";
            this.data = p_data;
        }

    }
}