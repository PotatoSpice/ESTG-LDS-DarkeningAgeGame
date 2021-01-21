namespace GameWebAPI.Models
{
    public class PlayerFriendInfo
    {
        public string username { get; set; }
        public bool online { get; set; }

        public PlayerFriendInfo(string username, bool online)
        {
            this.username = username;
            this.online = online;
        }
    }
}
