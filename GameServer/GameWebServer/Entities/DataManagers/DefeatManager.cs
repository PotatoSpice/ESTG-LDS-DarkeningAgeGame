using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using GameWebServer.Entities.Game;

namespace GameWebServer.Entities
{
    public class DefeatManager
    {
        private List<DefeatConditions> conditions;
        private List<Faction> initialFactionInfo;

        public DefeatManager(List<Faction> factions)
        {
            conditions = loadDefeatConditions();
            initialFactionInfo = factions;
        }

        private List<DefeatConditions> loadDefeatConditions()
        {
            List<DefeatConditions> defeats;
            string path = Path.GetFullPath("~/GameFiles/defeatconditions.json").Replace("~\\", "");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                //Console.WriteLine(json);
                defeats = JsonConvert.DeserializeObject<List<DefeatConditions>>(json);
            }
            return defeats;
        }

        public bool defeatCondition(Faction faction, int regionCount, int factionIndex)
        {
            int condition = faction.defeatCondition;
            switch (condition)
            {
                case 0:
                    return totalDefeat(regionCount);
                case 1:
                    return manpowerCheck(faction, factionIndex);
                case 2:
                    return goldCheck(faction, factionIndex);
                case 3:
                    return foodCheck(faction, factionIndex); 
                default:
                    return false;
            }
        }

        private bool totalDefeat(int regionCount)
        {
            return regionCount == 0;
        }

        private bool manpowerCheck(Faction faction, int factionIndex)
        {
            return ((double)faction.manpower / (double)initialFactionInfo[factionIndex].manpower) < 0.7;
        }

        private bool goldCheck(Faction faction, int factionIndex)
        {
            return ((double)faction.gold / (double)initialFactionInfo[factionIndex].gold) < 0.7;
        }

        private bool foodCheck(Faction faction, int factionIndex)
        {
            return ((double) faction.food / (double) initialFactionInfo[factionIndex].food) < 0.7;
        }

        public List<DefeatConditions> GetConditions()
        {
            return conditions; 
        }

    }
}
