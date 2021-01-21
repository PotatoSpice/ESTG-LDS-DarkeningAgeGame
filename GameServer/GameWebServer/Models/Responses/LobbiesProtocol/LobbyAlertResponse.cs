namespace GameWebServer.Models.Responses
{
    public class LobbyAlertResponse
    {
        public string eventType { get; private set; }
        public string title { get; set; }  
        public string data { get; set; }  

        public LobbyAlertResponse(string p_title, string p_data)
        {
            this.eventType = "lobby-alert";
            this.title = p_title;
            this.data = p_data;
        }

    }
}