using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameWebAPI.Entities
{
    public class FriendInvite
    {
        [Key]
        [ForeignKey("Players")]
        public string playerId { get; set; }
        [Key]
        [ForeignKey("Players")]
        public string targetPlayerId { get; set; }
    }
}
