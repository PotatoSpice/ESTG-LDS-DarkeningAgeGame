using System.Collections.Generic;

namespace GameWebServer.Entities.Game
{
    public class Army
    {

        public Army(string _name, General _general, string _region)
        {
            name = _name;
            general = _general;
            unitCount = 0;
            totalUnitCount = 0;
            region = _region;
            combatLock = false;
            units = new List<UnitArmy>(); 
        }

        public string name { get; set; }
        public General general { get; set; }
        public List<UnitArmy> units { get; set; }
        public string region { get; set; }
        public int unitCount { get; set; }
        public int totalUnitCount { get; set; }

        public bool combatLock { get; set; }

        public UnitArmy GetUnitArmy(string id)
        {
            return units.Find(u => u.id == id);
        }

    }
}
