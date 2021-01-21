using System.Collections.Generic;

namespace GameWebServer.Models.Responses
{
    public class IdFactionResponseEvent
    {

        public string eventType { get; set; }
        public List<IdFactionResponse> factionResponses { get; set; }
        public IdFactionResponseEvent(string _event, List<IdFactionResponse> _factionResponses)
        {
            eventType = _event;
            factionResponses = _factionResponses;
        }
    }
}
