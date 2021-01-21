using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using GameWebServer.Entities.Game;
using GameWebServer.Exceptions;

namespace GameWebServer.Entities
{
    public class FactionManager
    {
        private List<Faction> factions;
        private List<FactionsAtWar> wars;

        public FactionManager()
        {
            factions = loadFactions();
            wars = loadFactionsWar();
        }

        private List<Faction> loadFactions()
        {
            List<Faction> factions;
            string path = Path.GetFullPath("~/GameFiles/factions.json").Replace("~\\", "");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                //Console.WriteLine(json);
                factions = JsonConvert.DeserializeObject<List<Faction>>(json);
            }
            return factions;
        }

        private List<FactionsAtWar> loadFactionsWar()
        {
            List<FactionsAtWar> wars = new List<FactionsAtWar>();
            for (int i = 0; i < factions.Count; i++)
                for (int j = i + 1; j < factions.Count; j++)
                    wars.Add(new FactionsAtWar(factions[i].name, factions[j].name));

            return wars;
        }

        public Faction factionByName(string faction)
        {
            return factions.Find(f => f.name == faction);
        }
        public int factionByNameIndex(string faction)
        {
            return factions.FindIndex(0, f => f.name == faction);
        }


        public List<Faction> getFactions()
        {
            return factions;
        }

        public List<FactionsAtWar> getWars()
        {
            return wars;
        }

        public void updateFactionResources(string faction, int gold, int manpower, int wood, int food)
        {
            int index = factionByNameIndex(faction);
            factions[index].gold += gold;
            factions[index].manpower += manpower;
            factions[index].wood += wood;
            factions[index].food += food;
        }

        public bool factionsDeclareWar(string faction1, string faction2)
        {
            if (factionFirst(faction1, faction2))
            {
                return declareWar(faction1, faction2);
            }
            else
            {
                return declareWar(faction2, faction1);
            }
        }

        private bool declareWar(string faction1, string faction2)
        {
            return getWarInstance(faction1, faction2).active = true;
        }

        public bool factionsDeclareArmistice(string declaringFaction, string faction2)
        {
            FactionsAtWar war;
            if (factionFirst(declaringFaction, faction2))
                war = getWarInstance(declaringFaction, faction2);
            else
                war = getWarInstance(faction2, declaringFaction);

            return declarePeace(declaringFaction, war);
        }
        private bool declarePeace(string faction1, FactionsAtWar war)
        {
            war.active = false;
            return factionByName(faction1).armistice = true;
        }

        public bool areFactionsWarring(string faction1, string faction2)
        {
            if (factionFirst(faction1, faction2))
            {
                return factionsWar(faction1, faction2);
            }
            else
            {
                return factionsWar(faction2, faction1);
            }
        }

        private bool factionsWar(string faction1, string faction2)
        {
                FactionsAtWar warInstance = getWarInstance(faction1, faction2);
                return warInstance.active;
        }

        private FactionsAtWar getWarInstance(string faction1, string faction2)
        {
            foreach (FactionsAtWar war in wars)
            {
                Console.WriteLine(war.faction1 + "; " + faction2);
                Console.WriteLine(war.faction2 + "; " + faction1);
                if (war.faction1.Equals(faction1) && war.faction2.Equals(faction2))
                {
                    return war;
                }
            }
            return null;
        }

        private bool factionFirst(string faction1, string faction2)
        {
            if (factionByNameIndex(faction1) < factionByNameIndex(faction2)) return true;
            else return false;
        }

        public bool factionHasUsedPeace(string faction)
        {
            return factionByName(faction).armistice;
        }


        //DEFEAT CONDITIONS 

        public int checkFactionsDefeat(List<Region> capitals, DefeatManager defeatManager, List<int> regionCount)
        {
            int defeatCounter = 0;
            int ix = 0; 
            foreach (Faction factionL in factions)
            {
                foreach (Region capital in capitals)
                {
                    if (isFactionDefeated(factionL, capital, defeatManager, regionCount[ix]))
                    {
                        defeatCounter++;
                    }
                }
                ix++; 
            }
            return defeatCounter;
        }

        private bool isFactionDefeated(Faction factionL, Region capital, DefeatManager defeatManager, int regionCount) //ALL DEFEAT CONDITIONS SHOULD BE HERE
        {
            if (factionL.defeated == false)
            {
                    if (factionLostCapital(factionL, capital))
                    {
                        factionL.defeated = true;
                        return true;
                    }

                // return defeatManager.defeatCondition(factionL, regionCount, factionByNameIndex(factionL.name));
            }
            return false; 
        }

        private bool factionLostCapital(Faction faction, Region capital)
        {
            if (faction.capital.Equals(capital.name))
                if (!faction.name.Equals(capital.owner))
                    return true;
            return false;
        }

        public List<Faction> getDefeatedFactions()
        {
            List<Faction> dFactions = new List<Faction>();
            foreach (Faction faction in factions)
                if (faction.defeated == true)
                    dFactions.Add(faction);
            return dFactions; 
        }

        public List<Faction> getActiveFactions()
        {
            List<Faction> aFactions = new List<Faction>();
            foreach (Faction faction in factions)
                if (faction.defeated == false)
                    aFactions.Add(faction);
            return aFactions;
        }

        public bool capitulation(string faction)
        {
            int factionIndex = factionByNameIndex(faction);
            if (factions[factionIndex].defeated != true)
            {
                factions[factionIndex].defeated = true;
                return factions[factionIndex].defeated;
            }
            return false;
        }

    }
}
