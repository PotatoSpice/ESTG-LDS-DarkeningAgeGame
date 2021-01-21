using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.GameInvite
{
    public class GameInviteRequest
    {
        [Required]
        public string roomId { get; set; }
        [Required]
        public string invitedId { get; set; }
        [Required]
        [RegularExpression("Custom|Matchmaking", ErrorMessage = "The gameType must be either 'Custom' or 'Matchmaking' only.")]
        public string gameType { get; set; }
    }
}