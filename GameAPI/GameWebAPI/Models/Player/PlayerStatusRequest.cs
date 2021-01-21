using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Player
{
    public class PlayerStatusRequest
    {
        [Required]
        public bool status { get; set; }
    }
}