using System;
using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Player
{
    public class UpdatePlayerRequest
    {
        public string currentPassword { get; set; }
        
        [MinLength(6)]
        public string newPassword { get; set; }

        [EmailAddress]
        public string email { get; set; }

        public string firstName { get; set; }
        
        public string lastName { get; set; }
    }
}