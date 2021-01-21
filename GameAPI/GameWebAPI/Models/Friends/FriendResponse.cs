using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models
{
    public class FriendResponse
    {
        [Required]
        public string username { get; set; }

        [Range(typeof(bool), "false", "true", ErrorMessage="The field Is Active must be checked.")]
        public bool response { get; set; }
    }
}
