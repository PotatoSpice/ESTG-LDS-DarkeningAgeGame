namespace GameWebServer.Models.Requests
{
    public class LobbyRequest
    {
        public string eventType { get; set; }
        public string data { get; set; }

        public LobbyRequest(string p_eventType, string p_data)
        {
            this.eventType = p_eventType;
            this.data = p_data;
        }

    }
}