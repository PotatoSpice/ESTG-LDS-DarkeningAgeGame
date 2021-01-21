using GameWebAPI.Models.Player;

namespace GameWebAPI.Models.Auth
{
    public class AuthResponse
    {
        public string token { get; set; }
        public PlayerDTO playerData { get; set; }

        public AuthResponse(string token, PlayerDTO playerData)
        {
            this.token = token;
            this.playerData = playerData;
        }
    }
}