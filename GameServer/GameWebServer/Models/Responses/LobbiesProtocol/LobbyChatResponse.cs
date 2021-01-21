namespace GameWebServer.Models.Responses
{
    public class LobbyChatResponse
    {
        public string eventType { get; private set; }
        public string user { get; set; }  
        public string msg { get; set; }  
        public string time { get; set; } 

        public LobbyChatResponse(string p_user, string p_msg, string p_time)
        {
            this.eventType = "chat-message";
            this.user = p_user;
            this.msg = p_msg;
            this.time = p_time;
        }

    }
}