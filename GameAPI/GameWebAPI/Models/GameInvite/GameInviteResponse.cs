using System;

namespace GameWebAPI.Models.GameInvite
{
    public class GameInviteResponse
    {
        public string roomId { get; set; }
        public string hostId { get; set; }
        public DateTime createDate { get; set; }
        public string gameType { get; set; }
    }
}