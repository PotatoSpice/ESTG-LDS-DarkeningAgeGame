using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameWebAPI.Entities
{
    public class PlayerFriend
    {
        [Key]
        [ForeignKey("Players")]
        public string playerId { get; set; }
        [Key]
        [ForeignKey("Players")]
        public string friendId { get; set; }

        public PlayerFriend(string playerId, string friendId){
            this.playerId = playerId;
            this.friendId = friendId;
        }
    }
}