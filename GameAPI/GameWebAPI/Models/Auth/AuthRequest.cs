using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Auth
{
    public class AuthRequest
    {
        [Required]
        public string username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}