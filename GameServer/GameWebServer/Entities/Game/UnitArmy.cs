using System;

namespace GameWebServer.Entities.Game
{
    public class UnitArmy
    {
        public UnitArmy(Unit _unit, string _id)
        {
            unit = _unit;
            id = _id;
            availableManPower = _unit.maxManpower;
            reinforcementCosts = 0; 
        }

        public Unit unit { get; set; }
        public int availableManPower { get; set; }
        public String id { get; set; }

        public int reinforcementCosts { get; set; }

    }
}
