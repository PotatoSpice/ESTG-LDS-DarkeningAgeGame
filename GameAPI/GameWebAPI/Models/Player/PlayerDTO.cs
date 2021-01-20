namespace GameWebAPI.Models.Player
{
    public class PlayerDTO
    {
        public string username { get; set; }
        public string email { get; set; }

        public PlayerDTO(string username, string email)
        {
            this.username = username;
            this.email = email;
        }
    }
}