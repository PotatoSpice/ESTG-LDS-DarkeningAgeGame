using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebAPI.Models.PlayerMatchdata
{
    public class PlayerMatchdataRequest
    {

        /* public PlayerMatchdataRequest(string _gameID, string _playerId, int _placement, int _armiesCreated, int _regionsConquered)
        {
            gameID = _gameID; 
            playerId = _playerId;
            placement = _placement;
            armiesCreated = _armiesCreated;
            regionsConquered = _regionsConquered;
        } */

        public string gameID { get; set; }
        public string playerId { get; set; }
        public int placement { get; set; }
        public int armiesCreated { get; set; }
        public int regionsConquered { get; set; }

    }
}
