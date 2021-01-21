using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class General
{
    public bool available { get; set; }
    public string name { get; set; }
    public string faction { get; set; }
    public float bonus { get; set; }
    public int goldCost { get; set; }
    public Sprite image { get; set; }

    public string PrintGeneral()
    {
        return "available: " + available + "\n" +
            "name: " + name + "\n" +
            "faction: " + faction + "\n" +
            "bonus: " + bonus + "\n" +
            "goldCost: " + goldCost;
    }

    public void SetImage(string spriteName)
    {
        for(int i = 1; i <= 5; i++)
        {
            if (spriteName.Contains("" + i))
            {
                image = Resources.Load<Sprite>("generals/Tier"+i);
            }
        }
    }
}
