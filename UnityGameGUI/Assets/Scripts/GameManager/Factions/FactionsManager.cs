using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionsManager
{
    public string myFaction;
    public Faction[] factions;
    public List<string> updates;

    public FactionsManager()
    {
        this.factions = new Faction[4];
        this.updates = new List<string>();
        InitFactions();
    }

    private void InitFactions()
    {
        for (int i = 0; i < 4; i++)
        {
            Faction fac = new Faction();
            factions[i] = fac;
        }
        string[] factionsNames = { "Remnant", "Confederation", "Royalists", "Horde" };
        for (int i = 0; i < 4; i++)
        {
            this.factions[i].name = factionsNames[i];
            for (int j = 0; j < 4; j++)
            {
                if(this.factions[i].name != factionsNames[j])
                {
                    Dictionary<string, bool> value = new Dictionary<string, bool>();
                    value.Add("CanAttack", false);
                    value.Add("DeclaredThisRound", false);
                    value.Add("AttackedThisRound", false);
                    this.factions[i].warWith.Add(factionsNames[j], value);
                }
            }
        }
    }

    public void SetFactionInformation(JObject jsonResponse, bool inicialResponse)
    {
        foreach (object regionObj in jsonResponse["factions"])
        {
            JObject regionJson = JObject.Parse(regionObj.ToString());

            foreach (Faction fac in this.factions)
            {
                if (fac.name == regionJson["name"].ToString())
                {
                    fac.affinity = regionJson["affinity"].ToString();
                    fac.capital = regionJson["capital"].ToString();
                    fac.defeated = bool.Parse(regionJson["defeated"].ToString());
                    fac.manpower = int.Parse(regionJson["manpower"].ToString());
                    fac.wood = int.Parse(regionJson["wood"].ToString());
                    fac.gold = int.Parse(regionJson["gold"].ToString());
                    fac.food = int.Parse(regionJson["food"].ToString());
                    fac.armyCount = int.Parse(regionJson["armyCount"].ToString());
                    fac.armistice = bool.Parse(regionJson["armistice"].ToString());
                    fac.defeatConditionIndex = int.Parse(regionJson["defeatCondition"].ToString());
                }
            }
        }

        if (inicialResponse)
        {
            foreach (object regionObj in jsonResponse["idFactions"])
            {
                JObject regionJson = JObject.Parse(regionObj.ToString());

                foreach (Faction fac in this.factions)
                {
                    if (fac.name == regionJson["playerFaction"].ToString())
                    {
                        fac.player = regionJson["playerId"].ToString();
                    }
                }
            }

            foreach (object regionObj in jsonResponse["defeatConditions"])
            {
                JObject regionJson = JObject.Parse(regionObj.ToString());

                foreach (Faction fac in this.factions)
                {
                    if (fac.defeatConditionIndex == int.Parse(regionJson["id"].ToString()))
                    {
                        fac.defeatConditionValue.Add("designation", regionJson["designation"].ToString());
                        fac.defeatConditionValue.Add("description", regionJson["description"].ToString());
                    }
                }
            }
        }
    }

    public void SetFactionMaintenance(JObject jsonResponse)
    {
        foreach (object regionObj in jsonResponse["factionMaintenances"])
        {
            JObject regionJson = JObject.Parse(regionObj.ToString());

            foreach (Faction fac in this.factions)
            {
                if (fac.name == regionJson["faction"].ToString())
                {
                    fac.foodMaintenance = int.Parse(regionJson["food"].ToString());
                    fac.goldMaintenance = int.Parse(regionJson["gold"].ToString());
                    fac.woodMaintenance = int.Parse(regionJson["wood"].ToString());
                }
            }
        }
    }

    public void SetFactionsWarsStates(JObject jsonResponse, bool updateRound)
    {
        foreach (object warObj in jsonResponse["wars"])
        {
            JObject warJson = JObject.Parse(warObj.ToString());
            foreach (Faction fac in factions)
            {
                if (fac.name == warJson["faction1"].ToString())
                {
                    if (updateRound)
                    {
                        SetRoundUpdate(fac, warJson, "faction2");
                    }
                    fac.warWith[warJson["faction2"].ToString()]["CanAttack"] = Convert.ToBoolean(warJson["active"].ToString());
                    fac.warWith[warJson["faction2"].ToString()]["DeclaredThisRound"] = false;
                    fac.warWith[warJson["faction2"].ToString()]["AttackedThisRound"] = false;
                }
                if (fac.name == warJson["faction2"].ToString())
                {
                    if (updateRound)
                    {
                        SetRoundUpdate(fac, warJson, "faction1");
                    }
                    fac.warWith[warJson["faction1"].ToString()]["CanAttack"] = Convert.ToBoolean(warJson["active"].ToString());
                    fac.warWith[warJson["faction1"].ToString()]["DeclaredThisRound"] = false;
                    fac.warWith[warJson["faction1"].ToString()]["AttackedThisRound"] = false;
                }
            }
        }
    }

    private void SetRoundUpdate(Faction fac, JObject warJson, string faction)
    {
        if (fac.name == InfoManager.Instance.factionsManager.myFaction)
        {
            if (fac.warWith[warJson[faction].ToString()]["DeclaredThisRound"] == true)
            {
                if (fac.warWith[warJson[faction].ToString()]["CanAttack"] == Convert.ToBoolean(warJson["active"].ToString()))
                {
                    if (Convert.ToBoolean(warJson["active"].ToString()))
                    {
                        InfoManager.Instance.factionsManager.updates.Add("You declared war to " + fac.warWith[warJson[faction].ToString()]);
                    }
                    else
                    {
                        InfoManager.Instance.factionsManager.updates.Add("You declared peace to " + fac.warWith[warJson[faction].ToString()]);
                    }
                }
            }
            else
            {
                if (fac.warWith[warJson[faction].ToString()]["CanAttack"] != Convert.ToBoolean(warJson["active"].ToString()))
                {
                    if (Convert.ToBoolean(warJson["active"].ToString()))
                    {
                        InfoManager.Instance.factionsManager.updates.Add(fac.warWith[warJson[faction].ToString()] + " declared war to you");
                    }
                    else
                    {
                        InfoManager.Instance.factionsManager.updates.Add(fac.warWith[warJson[faction].ToString()] + " declared peace to you");
                    }
                }
            }
        }
    }

    public void AddArmies(JObject jsonResponse)
    {
        updates.Clear();
        List<Army> myArmies = new List<Army>();
        foreach (Faction fac in this.factions)
        {
            if(fac.name == this.myFaction)
            {
                myArmies = fac.armies.ToList();
            }
            fac.armies.Clear();
        }

        foreach (object armyObj in jsonResponse["armies"])
        {
            JObject armyJson = JObject.Parse(armyObj.ToString());
            Army army = new Army();
            army = armyJson.ToObject<Army>();

            army.general.SetImage("General_" + army.general.name);
            foreach (UnitArmy unit in army.units)
            {
                unit.unit.SetImage("Unit_" + unit.unit.name);
                unit.reinforcing = false;
            }

            foreach (Faction fac in this.factions)
            {
                if (fac.name == army.general.faction)
                {
                    fac.armies.Add(army);
                }
            }
        }

        List<Army> newArmies = new List<Army>();
        foreach (Faction fac in this.factions)
        {
            if (fac.name == this.myFaction)
            {
                newArmies = fac.armies.ToList();
            }
        }

        if(myArmies.Count >= 0)
        {
            foreach (Army ar in myArmies)
            {
                bool found = false;
                for (int i = 0; i < newArmies.Count; i++)
                {
                    if (ar.name == newArmies[i].name)
                    {
                        found = true;
                        if (ar.unitCount < newArmies[i].unitCount)
                        {
                            if (ar.attackRound)
                            {
                                updates.Add("The army " + ar.name + " was damaged during your attack");
                            }
                            else
                            {
                                updates.Add("The enemy damaged the army " + ar.name);
                            }
                        }
                    }
                }
                if (!found)
                {
                    if (ar.attackRound)
                    {
                        updates.Add("The army " + ar.name + " was destroyed during your attack");
                    }
                    else
                    {
                        updates.Add("The enemy destroyed the army " + ar.name);
                    }
                }
            }
        }

        foreach (object armyMainObj in jsonResponse["armiesMaintenance"])
        {
            JObject armyMainJson = JObject.Parse(armyMainObj.ToString());

            foreach (Faction fac in this.factions)
            {
                foreach (Army ar in fac.armies)
                {
                    if (armyMainJson["army"].ToString() == ar.name)
                    {
                        ar.goldMaintenance = int.Parse(armyMainJson["goldMaintenance"].ToString());
                        ar.foodMaintenance = int.Parse(armyMainJson["foodMaintenance"].ToString());
                    }
                }
            }
        }
    }
}

