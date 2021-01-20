using System;
using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Entities
{
    public class Player
    {
        [Key]
        public string username { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        
        public byte[] passwordHash { get; set; }
        public byte[] passwordSalt { get; set; }

        public string pwdResetToken { get; set; }
        public DateTime? pwdResetTokenExpires { get; set; }

        public bool online { get; set; }
    }
}