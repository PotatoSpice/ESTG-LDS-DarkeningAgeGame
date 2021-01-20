using System;
using System.ComponentModel.DataAnnotations;

namespace GameWebAPI.Models.Player
{
    public class PlayerModel
    {
        public string username { get; set; }

        public string email { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }
    }
}