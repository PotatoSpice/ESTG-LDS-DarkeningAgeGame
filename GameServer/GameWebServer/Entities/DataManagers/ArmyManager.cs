using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using GameWebServer.Entities.Game;

namespace GameWebServer.Entities
{
    public class ArmyManager
    {
        private List<Army> armies;
        private List<ArmyMaintenance> armiesMaintenance;
        private List<General> generals;
        private List<Unit> units;

        public ArmyManager()
        {
            armies = new List<Army>();
            armiesMaintenance = new List<ArmyMaintenance>();
            generals = loadGenerals();
            units = loadUnits();
        }

        public List<Army> getArmies()
        {
            return armies;
        }

        public List<ArmyMaintenance> GetMaintenances()
        {
            return armiesMaintenance;
        }

        public List<General> getGenerals()
        {
            return generals;
        }

        public List<Unit> getUnits()
        {
            return units;
        }

        public List<General> loadGenerals()
        {
            List<General> generals;
            string path = Path.GetFullPath("~/GameFiles/generals.json").Replace("~\\", "");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                //Console.WriteLine(json);
                generals = JsonConvert.DeserializeObject<List<General>>(json);
            }
            return generals;

        }

        public List<Unit> loadUnits()
        {
            List<Unit> units;
            string path = Path.GetFullPath("~/GameFiles/units.json").Replace("~\\", "");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                //Console.WriteLine(json);
                units = JsonConvert.DeserializeObject<List<Unit>>(json);
            }
            return units;
        }

        public General getGeneralByName(string generalName)
        {
            return generals.Find(g => g.name == generalName);
        }

        public Unit getUnitByName(string unitName)
        {
            return units.Find(u => u.name == unitName);
        }

        public Army getArmyByName(string armyName)
        {
            return armies.Find(a => a.name == armyName);
        }

        public int getArmyIndex(string armyName)
        {
            return armies.FindIndex(0, a => a.name == armyName);
        }

        public bool RegionHasArmy(string region)
        {
            foreach (Army army in armies)
                if (army.region == region)
                    return true;
            return false;
        }

        public Army getArmyInRegion(string region)
        {
            return armies.Find(a => a.region == region);
        }

        public int getArmyInRegionIndex(string region)
        {
            return armies.FindIndex(0, a => a.region == region);
        }

        public Army createArmy(string faction, string name, General general, string region)
        {
            //General nGeneral = getGeneralByName(general);
            if (general.available)
                if (general.faction == faction && getArmyByName(name) == null)
                {
                    Army nArmy = new Army(name, general, region);
                    ArmyMaintenance nMaintenance = new ArmyMaintenance(name);
                    armies.Add(nArmy);
                    armiesMaintenance.Add(nMaintenance);
                    general.available = false;
                    return nArmy;
                }
            return null;
        }

        public bool removeArmy(string name, string faction, string general)
        {
            General nGeneral = getGeneralByName(general);
            if (nGeneral.faction == faction)
            {
                getGeneralByName(general).available = true; //General returns to active duty now
                //generals.Remove(getGeneralByName(general));
                return armies.Remove(getArmyByName(name));
            }
            return false;
        }

        //Eventually this method will have more handlers, namely those about combat and regarding time!
        public bool moveArmyIntoRegion(string army, string region, string faction)
        {
            if (RegionHasArmy(region))
                return false;

            int armyIndex = getArmyIndex(army);
            armies[armyIndex].region = region;

            return true;
        }

        public bool switchArmiesRegion(string army, string region, string faction)
        {

            if (RegionHasArmy(region))
            {
                int army1Index = getArmyIndex(army);
                int army2Index = getArmyInRegionIndex(region);
                string region1 = armies[army1Index].region;
                string region2 = armies[army2Index].region;
                armies[army1Index].region = region2;
                armies[army2Index].region = region1;
                return true;
            }
            return false;
        }

        private string createUnitID(string armyName, string unitName, int unitCount)
        {
            return armyName + "-" + unitName + (unitCount + 1);
        }

        public UnitArmy createUnit(Unit unit, string faction, string armyName)
        {
            int toAddArmyIndex = getArmyIndex(armyName);
            if (armies[toAddArmyIndex].general.faction == faction && armies[toAddArmyIndex].unitCount <= 15 && armies[toAddArmyIndex].combatLock == false)
            {
                UnitArmy unitArmy = new UnitArmy(unit, createUnitID(armyName, unit.name, armies[toAddArmyIndex].totalUnitCount));
                Console.WriteLine(unitArmy.unit.name + "; " + unitArmy.id);
                armies[toAddArmyIndex].units.Add(unitArmy);
                armies[toAddArmyIndex].unitCount++;
                armies[toAddArmyIndex].totalUnitCount++;
                armiesMaintenance[toAddArmyIndex].foodMaintenance += unitArmy.unit.foodMaintenance;
                armiesMaintenance[toAddArmyIndex].goldMaintenance += unitArmy.unit.goldMaintenance;
                return unitArmy;
            }

            return null;
        }

        public bool removeUnit(string unitID, string army)
        {
            int armyIndex = getArmyIndex(army);
            if (armies[armyIndex].units.Remove(armies[armyIndex].GetUnitArmy(unitID)))
            {
                armies[armyIndex].unitCount--;
                return true;
            }
            return false; //Can't remove unit
        }

        public int reinforceUnits(string unitID, string army)
        {

            int armyIndex = getArmyIndex(army);
            int maxPower = armies[armyIndex].GetUnitArmy(unitID).unit.maxManpower;
            int formerPower = armies[armyIndex].GetUnitArmy(unitID).availableManPower;
            armies[armyIndex].GetUnitArmy(unitID).availableManPower = armies[armyIndex].GetUnitArmy(unitID).unit.maxManpower;
            return maxPower - formerPower;
        }

        public void updateArmyMaintenance(string army)
        {
            int armyIndex = getArmyIndex(army);
            int goldTotal = 0;
            int foodTotal = 0;
            foreach (UnitArmy unit in armies[armyIndex].units)
            {
                // proportion = unit.availableManPower / unit.unit.maxManpower;
                goldTotal += (int)(unit.unit.goldMaintenance);
                foodTotal += (int)(unit.unit.foodMaintenance);
            }
            armiesMaintenance[armyIndex].goldMaintenance = goldTotal;
            armiesMaintenance[armyIndex].foodMaintenance = foodTotal;

        }

        public void removeArmyMaintenance(string army)
        {
            int armyIndex = getArmyIndex(army);
            armiesMaintenance.RemoveAt(armyIndex); 
        }

        //Will destroy Units in certain order
        public double armyDestruction(Army army, double percentage)
        {
            int manpowerToDrain = Convert.ToInt32(totalArmyManpower(army) * percentage);
            int drainedManpower = 0;
            Random rnd = new Random();


            while (drainedManpower != manpowerToDrain)
            {
                int index = rnd.Next(0, army.unitCount-1);
                if(army.units[index].availableManPower <= manpowerToDrain)
                {
                    army.units[index].availableManPower = 0;
                    removeUnit(army.units[index].id, army.name);
                    drainedManpower += army.units[index].availableManPower;
                }
                else
                {
                    army.units[index].availableManPower -= manpowerToDrain;
                    drainedManpower += manpowerToDrain;
                }
            }

            return drainedManpower;
            
        }


        public int combatMethod(List<TerrainBonus> terrainBonus, RegionType regionT, string attackArmy, string defenceArmy
            , string attackerAffinity, string defenderAffinity)
        {
            Console.WriteLine(1);
            Army attackArmyI = getArmyByName(attackArmy);
            Army defenceArmyI = getArmyByName(defenceArmy);
            double attackArmyPower = calculateArmyPower(attackArmyI, true, attackerAffinity, regionT, terrainBonus);
            double defenceArmyPower = calculateArmyPower(defenceArmyI, false, defenderAffinity, regionT, terrainBonus);

            double totalPower = attackArmyPower + defenceArmyPower;
            double attackPercentage = attackArmyPower / totalPower * 100;
            if (attackPercentage > 70)
            {
                Console.WriteLine(2);
                removeArmyMaintenance(attackArmy);
                removeArmy(defenceArmy, defenceArmyI.general.faction, defenceArmyI.general.name);
                return 1;
            } //Attack Army great win
            else if (attackPercentage > 55)
            {
                Console.WriteLine(3);
                armyDestruction(attackArmyI, attackPercentage);
                armyDestruction(defenceArmyI, attackPercentage+30);
                updateArmyMaintenance(attackArmy);
                updateArmyMaintenance(defenceArmy);
                return 2;
            } //attack army win
            else if (attackPercentage <= 55 && attackPercentage >= 45)
            {
                Console.WriteLine(4);
                armyDestruction(attackArmyI, attackPercentage);
                armyDestruction(defenceArmyI, attackPercentage);
                updateArmyMaintenance(attackArmy);
                updateArmyMaintenance(defenceArmy);
                return 3;
            } //draw
            else if (attackPercentage > 30 && attackPercentage < 55)
            {

                Console.WriteLine(5);
                armyDestruction(attackArmyI, attackPercentage + 30);
                armyDestruction(defenceArmyI, attackPercentage);
                updateArmyMaintenance(attackArmy);
                updateArmyMaintenance(defenceArmy);
                return 4;
            }//defender win
            else
            {
                Console.WriteLine(6);
                removeArmyMaintenance(attackArmy);
                removeArmy(attackArmy, attackArmyI.general.faction, attackArmyI.general.name); 
                return 5; 
            } //attackerArmyAnihilated
        }

        private double unitPowerCalculator(UnitArmy unitA, General general, bool attacking, List<TerrainBonus> bonus)
        {
            double total = 0;
            for (int i = 0; i < 3; i++)
            {
                double unitTerrainBonus = regionBonus(unitA.id, bonus);
                if (attacking == true)
                {
                    if (i == 0)
                    {
                        total += (unitA.unit.attackEarly * general.bonus * unitTerrainBonus);
                    }
                    if (i == 1)
                    {
                        total += (unitA.unit.attackMid * general.bonus * unitTerrainBonus);
                    }
                    if (i == 2)
                    {
                        total += (unitA.unit.attackLate * general.bonus * unitTerrainBonus);
                    }

                }
                else
                {
                    if (i == 0)
                    {
                        total += (unitA.unit.defenceEarly * general.bonus * unitTerrainBonus);
                    }
                    if (i == 1)
                    {
                        total += (unitA.unit.defenceMid * general.bonus * unitTerrainBonus);
                    }
                    if (i == 2)
                    {
                        total += (unitA.unit.defenceLate * general.bonus * unitTerrainBonus);
                    }
                }
            }

            total = (unitA.availableManPower * total);
            return total;
        }

        private double calculateArmyPower(Army army, bool attacking, string affinity, RegionType region, List<TerrainBonus> terrainBonus)
        { //Region is here because in the future I shall implement more material regarding this
            double total = 0;
            foreach (UnitArmy unit in army.units)
            {
                total += unitPowerCalculator(unit, army.general, attacking, terrainBonus);
            }

            if (attacking == true)
            {
                if (affinity == "Offence")
                {
                    total = total * (1 + 0.5); //more stuff shall be implemented here later
                }
            }
            else
            {
                if (affinity == "Defence")
                {
                    total = total * (1 + 0.5 + region.defencebonus);
                }
                else
                {
                    total = total * (1 + region.defencebonus);
                }
            }
            return total;
        }

        private int totalArmyManpower(Army army)
        {
            int total = 0;
            foreach (UnitArmy unit in army.units)
                total += unit.availableManPower;
            return total; 
        }

        private double regionBonus(string unit, List<TerrainBonus> terrainBonus)
        {
            foreach (TerrainBonus terrains in terrainBonus)
                if (terrains.unitType.Equals(unit))
                    return terrains.bonus;
            return 1;
        }

    }
}
