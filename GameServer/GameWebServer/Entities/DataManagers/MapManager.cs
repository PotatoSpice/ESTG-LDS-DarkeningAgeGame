using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameWebServer.Entities.Game;
using GameWebServer.Utils;

namespace GameWebServer.Entities
{

    /* This class will include all the methods in which the 
    *  Map is important
    *  That is, Army Movement and Transfers, Conquests, Annexations, etc.
    *  Moreover, where Region Information is required
    */
    public class MapManager
    {
        private MapGraph<Region> map;
        public List<Region> regions { get; private set; }
        public List<RegionType> regionTypes { get; private set; }
        public List<TerrainBonus> terrainBonus { get; private set; }
        public bool gameLoaded { get; set; }
        private Semaphore _pool;
        public MapManager() {
            this.gameLoaded = false;
            _pool = new Semaphore(1, 1);
        }

        //Instead of using the constructor to build the information, rather have methods now:

        public async Task loadGameInfo()
        {
            await Task.Run(() => loadMap());
            await Task.Run(() => loadRegions());
            await Task.Run(() => loadTerrains());
            this.gameLoaded = true;
        }

        private MapGraph<Region> loadMap()
        {
            map = MapReader.loadIntoGraph();
            regions = map.getVertex().ToList<Region>();
            return map;
        }

        private List<RegionType> loadRegions()
        {
            regionTypes = MapReader.loadRegionTypes();
            return regionTypes;
        }

        private List<TerrainBonus> loadTerrains()
        {
            terrainBonus = MapReader.loadTerrainBonus();
            return terrainBonus;
        }


        //Generic Information Methods :
        public List<Region> getRegionsByType(String type)
        {
            List<Region> regionsByType = new List<Region>();
            foreach (Region region in map.getVertex())
                if (region != null)
                    if (region.type.Equals(type))
                        regionsByType.Add(region);

            return regionsByType;
        }

        public RegionType getTypeInformations(String type)
        {
            foreach (RegionType tempType in regionTypes)
                if (tempType.type.Equals(type))
                    return tempType;
            return null;
        }

        public List<Region> getFactionRegions(String faction) //This can be used even with Neutral regions, therefore, there is 0 need to actually check for "neutral" regions, this suits fine.
        {
            List<Region> factionRegions = new List<Region>();
            foreach (Region region in map.getVertex())
                if (region != null)
                    if (region.owner.Equals(faction))
                        factionRegions.Add(region);
            return factionRegions;
        }

        //More important game logic stuff:
        public Boolean regionsHaveBorder(Region region1, Region region2)
        {
            return map.edgeBetweenVertexes(region1, region2);
        }

        public int travelingDistance(Region sourceRegion, Region destinyRegion)
        {
            if (regionsHaveBorder(sourceRegion, destinyRegion))
                return ((sourceRegion.size + destinyRegion.size) / 2);
            else return 0; //Anywhere it says that "distance" is 0, it means it doesn't exist. 
        }

        public List<Region> borderRegions(Region region)
        {
            return map.connectedVertexes(region);
        }

        public bool changeRegionOwner(string newowner, Region region)
        {
            int index = map.getIndex(region);
            if (map.indexIsValid(index))
            {
                Console.WriteLine("Old Owner: " + map.getVertex()[index].owner+ "\nNew owner:" + newowner);
                Console.WriteLine("Regions is:" + map.getVertex()[index].name);
                _pool.WaitOne();
                map.getVertex()[index].owner = newowner;
                _pool.Release(1);
                Console.WriteLine("Regions owner is:" + map.getVertex()[index].owner);
                return true;
            }
            return false;
        }

        //This method might look a bit "complicated", but it's actually a way to check if the Region does actually exist: otherwise it would be far easier, of course. 
        public string regionTerrain(Region region)
        {
            int index = map.getIndex(region);
            if (map.indexIsValid(index))
                return region.terrain;
            else return null;

        }

        public Region searchRegionByName(String regionName)
        {
            foreach (Region lookregion in map.getVertex())
                if (lookregion.name.Equals(regionName))
                    return lookregion;

            return null;
        }

        public RegionType searchRegionType(String typeName)
        {
            foreach (RegionType regionType in regionTypes)
                if (regionType.type.Equals(typeName))
                    return regionType;
            return null;
        }

        public double regionBonus(string unit, string terrain)
        {
            List<TerrainBonus> terrainBonus = getTerrainByType(terrain);
            foreach (TerrainBonus terrains in terrainBonus)
                if (terrains.unitType.Equals(unit))
                    return terrains.bonus;
            return 1; 
        }
        public List<TerrainBonus> getTerrainByType(string terrain)
        {
            List<TerrainBonus> returnTerrain = new List<TerrainBonus>();
            foreach (TerrainBonus terrains in terrainBonus)
                if (terrains.terrain.Equals(terrain))
                    returnTerrain.Add(terrains);
            return returnTerrain; 
        }

       public void updateRegions()
        {
            regions = map.getVertex().ToList<Region>();
        }

        //Procedure methods which might use the ones above as verifications 

    }
}
