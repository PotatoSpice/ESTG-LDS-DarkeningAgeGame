using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Player
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
    }
}