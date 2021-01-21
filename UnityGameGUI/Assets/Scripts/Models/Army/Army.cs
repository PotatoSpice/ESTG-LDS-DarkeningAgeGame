using System.Collections.Generic;

public class Army
{
    public string name { get; set; }
    public General general { get; set; }
    public List<UnitArmy> units { get; set; }
    public string region { get; set; }
    public int unitCount { get; set; }
    public int totalUnitCount { get; set; }
    public bool combatLock { get; set; }
    public int goldMaintenance { get; set; }
    public int foodMaintenance { get; set; }
    public bool attackRound { get; set; }

    public Army()
    {
        units = new List<UnitArmy>();
        attackRound = false;
    }

    public string PrintArmy()
    {
        return "name: " + name + "\n" +
            "general: " + general.name + "\n" +
            "region: " + region + "\n" +
            "unitCount: " + unitCount + "\n" +
            "totalUnitCount: " + totalUnitCount;
    }
}
