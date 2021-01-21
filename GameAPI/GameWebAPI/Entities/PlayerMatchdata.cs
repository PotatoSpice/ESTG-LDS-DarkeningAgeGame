using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebAPI.Entities
{
    public class PlayerMatchdata
    { 
        [Key]
        [ForeignKey("Players")]
        public string playerId { get; set; }
        [Key]
        public string gameID { get; set; }
        public int placement { get; set; }
        public int armiesCreated { get; set; }
        public int regionsConquered { get; set; }
        public DateTime date { get; set; }
    }
}
