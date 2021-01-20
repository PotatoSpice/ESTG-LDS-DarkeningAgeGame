using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models
{
    public class FriendInviteRequest
    {
        [Required]
        public string targetUsername { get; set; }
    }
}