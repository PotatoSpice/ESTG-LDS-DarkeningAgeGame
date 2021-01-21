using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using GameWebServer.Entities;
using GameWebServer.Entities.Game;

namespace GameWebServer.Utils
{
    public static class MapReader
    {

        // This method reads all game map related JSON information.
        public static List<Region> loadMapInfo()
        {
            List<Region> regions;
            string path = Path.GetFullPath("~/GameFiles/mapconfigs.json").Replace("~\\","");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
               // Console.WriteLine(json); 
                regions = JsonConvert.DeserializeObject<List<Region>>(json);
            }
            return regions; 
        }


        // This method loads the information pertained originally in JSON form and creates a graph with it. 
        public static MapGraph<Region> loadIntoGraph()
        {
            MapGraph<Region> graphRegions = new MapGraph<Region>(); 
            List<Region> regionsToAdd = loadMapInfo();
            foreach (Region region in regionsToAdd)
                graphRegions.addVertex(region);

            foreach (Region region in regionsToAdd)
                foreach (Region secondRegion in regionsToAdd)
                    foreach (String border in region.Borders)
                        if (border.Equals(secondRegion.name))
                            graphRegions.addEdge(region, secondRegion, region.size);

            return graphRegions; 
        }

        public static List<RegionType> loadRegionTypes()
        {
            List<RegionType> types;
            string path = Path.GetFullPath("~/GameFiles/regiontypes.json").Replace("~\\","");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
               // Console.WriteLine(json);
                types = JsonConvert.DeserializeObject<List<RegionType>>(json);
            }
            return types;
        }

        public static IList<Faction> readFactions()
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

        public static List<TerrainBonus> loadTerrainBonus()
        {
            List<TerrainBonus> types;
            string path = Path.GetFullPath("~/GameFiles/terrainbonus.json").Replace("~\\", "");
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                // Console.WriteLine(json);
                types = JsonConvert.DeserializeObject<List<TerrainBonus>>(json);
            }
            return types;
        }

    }
}
