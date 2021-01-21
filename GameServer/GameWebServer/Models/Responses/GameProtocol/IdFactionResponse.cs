namespace GameWebServer.Models.Responses
{
    public class IdFactionResponse
    {

        public IdFactionResponse(string _playerId, string _playerFaction)
        {
            playerId = _playerId;
            playerFaction = _playerFaction;
        }

        public string playerId { get; set; }
        public string playerFaction { get; set; }

    }
}
