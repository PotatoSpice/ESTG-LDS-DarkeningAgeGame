using System.Collections.Generic;

namespace GameWebServer.Models.Responses
{
    public class LobbyAlertExtendedResponse
    {
        public string eventType { get; private set; }
        public string title { get; set; }  
        public ICollection<string> data { get; set; }  

        public LobbyAlertExtendedResponse(string p_title, ICollection<string> p_data)
        {
            this.eventType = "lobby-alert-ext";
            this.title = p_title;
            this.data = p_data;
        }

    }
}