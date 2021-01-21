using System;
using System.Collections.Generic;

namespace GameWebServer.Models.Requests
{
    public class GameRequest
    {
        public String EventType { get; set; }
        public List<String> data { get; set; }  

    }
}
