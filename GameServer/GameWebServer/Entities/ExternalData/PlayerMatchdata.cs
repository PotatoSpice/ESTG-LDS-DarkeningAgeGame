namespace GameWebServer.Entities.ExternalData
{
    public class PlayerMatchdata
    {
        public PlayerMatchdata(string _gameID, string _playerId, int _placement, int _armiesCreated, int _regionsConquered)
        {
            gameID = _gameID; 
            playerId = _playerId;
            placement = _placement;
            armiesCreated = _armiesCreated;
            regionsConquered = _regionsConquered;
        }

        public string gameID { get; private set; }
        public string playerId { get; private set; }
        public int placement{ get; private set;}
        public int armiesCreated { get; private set; }
        public int regionsConquered { get; private set;  }

    }
}
