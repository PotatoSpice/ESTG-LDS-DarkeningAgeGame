using System;
using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Player
{
    public class SignUpRequest
    {
        [Required]
        public string username { get; set; }

        [Required]
        [MinLength(6)]
        public string password { get; set; }

        [Required]
        [Compare("password")]
        public string confirmPassword { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }
    }
}