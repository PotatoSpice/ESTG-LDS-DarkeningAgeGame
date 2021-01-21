using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class ArmyCollections
{
    private List<Unit> unitTypes;
    private List<General> generalsType;

    public ArmyCollections()
    {
        unitTypes = new List<Unit>();
        generalsType = new List<General>();
    }

    public void SetUnitsCollection(JObject jsonResponse)
    {
        foreach (object unitObj in jsonResponse["units"])
        {
            if (unitObj.ToString() != "")
            {
                try
                {
                    JObject unitJson = JObject.Parse(unitObj.ToString());
                    Unit unit = unitJson.ToObject<Unit>();
                    unit.SetImage("Unit_" + unit.name);
                    unitTypes.Add(unit);
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    public void SetGeneralsCollection(JObject jsonResponse)
    {
        generalsType.Clear();
        foreach (object generalObj in jsonResponse["generals"])
        {
            if (generalObj.ToString() != "")
            {
                try
                {
                    JObject generalJson = JObject.Parse(generalObj.ToString());
                    General general = generalJson.ToObject<General>();
                    general.SetImage("General_" + general.name);
                    generalsType.Add(general);
                }
                catch (Exception e)
                {
                }
            }
        }
    }

    public List<Unit> getUnits()
    {
        return unitTypes;
    }

    public List<General> getGeneral()
    {
        return generalsType;
    }
}

