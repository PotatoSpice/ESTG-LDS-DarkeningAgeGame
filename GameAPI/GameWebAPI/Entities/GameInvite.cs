using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GameWebAPI.Models.GameInvite;

namespace GameWebAPI.Entities
{
    public class GameInvite
    {
        
        [Key]
        public string roomId { get; set; }
        [Key]
        [ForeignKey("Players")]
        public string invitedId { get; set; }
        [ForeignKey("Players")]
        public string hostId { get; set; }
        public DateTime createDate { get; set; }
        public string gameType { get; set; }

        public GameInvite(string roomId, string invitedId, string hostId, string gameType){
            this.roomId = roomId;
            this.invitedId = invitedId;
            this.hostId = hostId;
            this.gameType = gameType;
            this.createDate = DateTime.Now;
        }
    }
}