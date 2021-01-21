using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class ServerResponse
    {
        public String EventType { get; set; }
        public List<String> data { get; set; }

        public ServerResponse()
        {
            data = new List<string>();
        }
    }
}
