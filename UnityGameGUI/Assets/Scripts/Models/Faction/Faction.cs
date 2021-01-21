using System.Collections.Generic;
using UnityEngine;

public class Faction
{
    public string name;
    public string player;
    public string affinity;
    public string capital;
    public bool defeated;
    public int manpower;
    public int wood;
    public int gold;
    public int food;
    public int armyCount;
    public int goldMaintenance;
    public int foodMaintenance;
    public int woodMaintenance;
    public int defeatConditionIndex;
    public Dictionary<string, string> defeatConditionValue;
    public bool armistice;
    public List<Army> armies;
    public Dictionary<string, Dictionary<string, bool>> warWith;
    //resources
    
    public Faction()
    {
        armies = new List<Army>();
        warWith = new Dictionary<string, Dictionary<string, bool>>();
        defeatConditionValue = new Dictionary<string, string>();
    }

    public void PrintFaction()
    {
        Debug.Log("Name: " + name + "\n" +
            "Player: " + player + "\n" +
            "Affinity: " + affinity + "\n" +
            "Capital: " + capital + "\n" +
            "Defeated: " + defeated + "\n" +
            "ManPower: " + manpower + "\n" +
            "Wood: " + wood + "\n" +
            "Gold: " + gold + "\n" +
            "Food: " + food + "\n" +
            "ArmyCount: " + armyCount + "\n" +
            "Gold Maintenance: " + goldMaintenance + "\n" +
            "Food Maintenance: " + foodMaintenance + "\n" +
            "Wood Maintenance: " + woodMaintenance);
    }
}