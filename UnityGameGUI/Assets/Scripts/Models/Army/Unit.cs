using UnityEngine;
using UnityEngine.UI;

public class Unit
{
    public string name { get; set; }
    public int maxManpower { get; set; }
    public int maxMovementSpeed { get; set; }
    public float attackEarly { get; set; }
    public float attackMid { get; set; }
    public float attackLate { get; set; }
    public float defenceEarly { get; set; }
    public float defenceMid { get; set; }
    public float defenceLate { get; set; }
    public int goldCost { get; set; }
    public int woodCost { get; set; }
    public int foodCost { get; set; }
    public int foodMaintenance { get; set; }
    public int goldMaintenance { get; set; }
    public Sprite image { get; set; }

    public string PrintUnit()
    {
        return "name: " + name + "\n" +
            "maxManpower: " + maxManpower + "\n" +
            "maxMovementSpeed: " + maxMovementSpeed + "\n" +
            "attackEarly: " + attackEarly + "\n" +
            "attackMid: " + attackMid + "\n" +
            "attackLate: " + attackLate + "\n" +
            "defenceEarly: " + defenceEarly + "\n" +
            "defenceMid: " + defenceMid + "\n" +
            "defenceLate: " + defenceLate + "\n" +
            "goldCost: " + goldCost + "\n" +
            "woodCost: " + woodCost + "\n" +
            "foodCost: " + foodCost + "\n" +
            "foodMaintenance: " + foodMaintenance + "\n" +
            "goldMaintenance: " + goldMaintenance;
    }

    public void SetImage(string spriteName)
    {
        image = Resources.Load<Sprite>("units/"+spriteName);
    }
}
