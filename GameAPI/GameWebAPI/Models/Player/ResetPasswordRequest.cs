using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Player
{
    public class ResetPasswordRequest
    {
        [Required]
        public string token { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        [Compare("password")]
        public string confirmPassword { get; set; }
    }
}