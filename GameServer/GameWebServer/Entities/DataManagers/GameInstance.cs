using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameWebServer.Models.Responses;
using GameWebServer.Services;
using System.Threading;
using GameWebServer.Models.Requests;
using GameWebServer.Entities.Game;
using System.Collections;
using GameWebServer.Entities.Room;
using GameWebServer.Entities.Player;
using GameWebServer.Utils;
using GameWebServer.Entities.ExternalData;
using System.Collections.Concurrent;
using System.Net.Http;

namespace GameWebServer.Entities
{
    public class GameInstance
    {
        private FactionManager factionManager;
        private List<FactionMaintenance> factionMaintenances;
        private ArmyManager armyManager;
        private int turn;
        private MapManager mapManager;
        private string roomId;
        private ConcurrentQueue<GameRequest> concurrentGameQueue;
        private ConcurrentQueue<string> concurrentPlayerQueue;
        private DefeatManager defeatManager;
        public bool editable;
        private int defeatedcount;
        public int playerCount { get; set; }
        public Timer turnTimer { get; set; }

        private Timer timer2;
        private Timer timer3;
        private int editableCounter;
        private bool canMakeEditable;

        public GameInstance(string _roomId)
        {
            armyManager = new ArmyManager();
            mapManager = new MapManager();
            factionManager = new FactionManager();
            factionMaintenances = openMaintenanceList();
            defeatManager = new DefeatManager(factionManager.getFactions());
            turn = 0;
            roomId = _roomId;
            concurrentGameQueue = new ConcurrentQueue<GameRequest>();
            concurrentPlayerQueue = new ConcurrentQueue<string>();
            editable = false;
            defeatedcount = 0;
            playerCount = 0;
            editableCounter = 0;
            canMakeEditable = true;
        }

        public bool IsGameLoaded()
        {
            return mapManager.gameLoaded;
        }

        public void setGameLoaded()
        {
            mapManager.gameLoaded = true;
        }

        /* public LinkedQueue<GameRequest> getQueue()
         {
             return gameQueue;
         }

         public LinkedQueue<string> getPlayerQueue()
         {
             return playerEventQueue; 
         } */

        public InitInformationResponse initialInformation(List<IdFactionResponse> idFactions)
        {
            InitInformationResponse initInformationModel = new InitInformationResponse(turn, mapManager.regions, mapManager.regionTypes, armyManager.getUnits(), armyManager.getGenerals(), factionManager.getFactions(), factionMaintenances, idFactions, factionManager.getWars(), defeatManager.GetConditions());
            return initInformationModel;
        }

        public UpdatedInformationResponse updatedInformationTurn()
        {
            mapManager.updateRegions(); 
            UpdatedInformationResponse updatedInformationModel = new UpdatedInformationResponse(turn, mapManager.regions, armyManager.getArmies(), armyManager.getGenerals(), factionManager.getFactions(), armyManager.GetMaintenances(), getfactionsMaintenance(), factionManager.getWars());
            return updatedInformationModel;
        }

        public async Task loadGameInfo()
        {
            await mapManager.loadGameInfo();
        }

        public bool createArmy(string faction, string general, string name, string region, string unit)
        {
            Console.WriteLine(1);
            Region objRegion = mapManager.searchRegionByName(region);
            if (objRegion.type == "Fort" || objRegion.type == "Urban" || objRegion.type == "Capital" || objRegion.type == "SpecialCapital" || factionManager.factionByName(faction).armyCount == 5)
            {
                General aGeneral = armyManager.getGeneralByName(general);
                Faction aFaction = factionManager.factionByName(faction);
                Console.WriteLine(2);
                if (aFaction.gold > aGeneral.goldCost)
                {
                    Console.WriteLine(3);
                    Army army = armyManager.createArmy(faction, name, aGeneral, region);
                    if (army != null)
                    {
                        Console.WriteLine(4);
                        Unit aUnit = armyManager.getUnitByName(unit);
                        if (aUnit.goldCost < aFaction.gold && aUnit.woodCost < aFaction.wood && aUnit.foodCost < aFaction.food)
                        {
                            Console.WriteLine(5);
                            UnitArmy cUnit = armyManager.createUnit(aUnit, faction, name);
                            factionManager.factionByName(faction).armyCount++;
                            if (cUnit != null)
                            {
                                Console.WriteLine(6);
                                factionManager.updateFactionResources(faction, -aUnit.goldCost, -aUnit.maxManpower, -aUnit.woodCost, -aUnit.foodCost);
                                updateMaintenance(name);
                                updateMaintenanceFaction(faction);
                                return true;
                            }
                        }
                    }
                }

            }
            return false;

        }

        public bool createUnit(string faction, string army, string unit, string region)
        {
            Region objRegion = mapManager.searchRegionByName(region);
            int armyIndex = armyManager.getArmyIndex(army);
            Faction aFaction = factionManager.factionByName(faction);
            if (armyIndex != -1)
                if (objRegion.type == "Fort" || objRegion.type == "Urban" || objRegion.type == "Capital" || objRegion.type == "SpecialCapital" && objRegion.owner == faction)
                {
                    Unit aUnit = armyManager.getUnitByName(unit);
                    if (aUnit.goldCost < aFaction.gold && aUnit.woodCost < aFaction.wood && aUnit.foodCost < aFaction.food)
                    {
                        UnitArmy cUnit = armyManager.createUnit(aUnit, faction, army);
                        if (cUnit != null)
                        {
                            factionManager.updateFactionResources(faction, -aUnit.goldCost, -aUnit.maxManpower, -aUnit.woodCost, -aUnit.foodCost);
                            updateMaintenance(army);
                            updateMaintenanceFaction(faction);
                            return true;
                        }
                    }
                }
            return false;
        }

        public bool moveArmy(string army, string region, string faction)
        {
            int armyIndex = armyManager.getArmyIndex(army);
            if (armyIndex != -1)
                if (mapManager.regionsHaveBorder(mapManager.searchRegionByName(armyManager.getArmyByName(army).region), mapManager.searchRegionByName(region)))
                    return armyManager.moveArmyIntoRegion(army, region, faction);
            return false;
        }

        public bool swapArmiesInRegions(string army, string region, string faction)
        {
            int armyIndex = armyManager.getArmyIndex(army);
            if (armyIndex != -1)
                return armyManager.switchArmiesRegion(army, region, faction);
            return false;
        }

        public int reinforceUnits(string unitID, string army)
        {
            int armyIndex = armyManager.getArmyIndex(army);
            if (armyIndex != -1)
            {
                updateMaintenance(army);
                updateMaintenanceFaction(armyManager.getArmies()[armyIndex].general.faction);
                return armyManager.reinforceUnits(unitID, army);
            }
            return -1;
        }

        private bool updateMaintenance(string army)
        {
            int armyIndex = armyManager.getArmyIndex(army);
            if (armyIndex != -1)
            {
                armyManager.updateArmyMaintenance(army);
                return true;
            }
            return false;
        }

        public void updateEveryMaintenance()
        {
            foreach (Army army in armyManager.getArmies())
                updateMaintenance(army.name);
        }

        private List<FactionMaintenance> openMaintenanceList()
        {
            List<FactionMaintenance> fm = new List<FactionMaintenance>();
            foreach (Faction faction in factionManager.getFactions())
                fm.Add(new FactionMaintenance(faction.name));
            return fm;
        }

        //Unsure if the next two methods should be here or on FactionManager, but ok
        private void updateMaintenanceFaction(string faction)
        {
            int index = factionMaintenances.FindIndex(f => f.faction == faction);
            int i = 0;
            factionMaintenances[index].gold = 0;
            factionMaintenances[index].food = 0;
            foreach (ArmyMaintenance maintenance in armyManager.GetMaintenances())
            {
                if (armyManager.getArmies()[i].general.faction == faction)
                {
                    factionMaintenances[index].gold += maintenance.goldMaintenance;
                    factionMaintenances[index].food += maintenance.foodMaintenance;
                }
                i++;
            }
        }

        private void updateAllFactionsMaintenance() //This will be used in combat related stuff
        {
            foreach (Faction faction in factionManager.getFactions())
            {
                updateMaintenanceFaction(faction.name);
            }
        }

        public List<FactionMaintenance> getfactionsMaintenance()
        {
            return factionMaintenances;
        }

        //War and conquest and stuff

        public bool declareWar(string faction, string faction2)
        {
            if (!factionManager.areFactionsWarring(faction, faction2))
                if (factionsHaveBorder(faction, faction2))
                    return factionManager.factionsDeclareWar(faction, faction2);
            return false;
        }
        public bool declarePeace(string faction, string faction2)
        {
            if (factionManager.areFactionsWarring(faction, faction2))
                if (!factionManager.factionHasUsedPeace(faction))
                    return factionManager.factionsDeclareArmistice(faction, faction2);
            return false;
        }

        private bool factionsHaveBorder(string faction, string faction2)
        {
            List<Region> faction1Regions = mapManager.getFactionRegions(faction);
            List<Region> faction2Regions = mapManager.getFactionRegions(faction2);

            foreach (Region region in faction1Regions)
                foreach (Region region2 in faction2Regions)
                    if (mapManager.regionsHaveBorder(region, region2))
                        return true;
            return false;
        }

        public bool attackRegion(string attackerregion, string defenderregion, string attackarmy, string defencearmy)
        {

            Region launchedRegion = mapManager.searchRegionByName(attackerregion);
            Region targetRegion = mapManager.searchRegionByName(defenderregion);
            if (launchedRegion.owner.Equals(targetRegion.owner))
                return false;
            if (!factionManager.areFactionsWarring(launchedRegion.owner, targetRegion.owner))
            {
                return false;
            }
            else
            if (defencearmy != "")
            {
                Console.WriteLine(targetRegion.owner);
                string attackeraffinity = factionManager.factionByName(launchedRegion.owner).affinity;
                string defenderaffinty = factionManager.factionByName(targetRegion.owner).affinity;
                int attackOutcome = -1;
                Console.WriteLine((armyManager.getArmyIndex(attackarmy)));
                Console.WriteLine((armyManager.getArmyIndex(defencearmy)));
                if (armyManager.getArmyIndex(attackarmy) != -1 && armyManager.getArmyIndex(defencearmy) != -1) //Unsure if more verifications are necessary
                    attackOutcome = armyManager.combatMethod(mapManager.getTerrainByType(targetRegion.terrain), mapManager.getTypeInformations(targetRegion.type), attackarmy, defencearmy, attackeraffinity, defenderaffinty);

                if (attackOutcome == -1)
                {
                    Console.WriteLine("false");
                    return false;
                }
                else
                {
                    Console.WriteLine("true");
                    if (attackOutcome == 1)
                    {
                        mapManager.changeRegionOwner(launchedRegion.owner, targetRegion);
                        moveArmy(attackarmy, defenderregion, launchedRegion.owner);
                    }
                    else if (attackOutcome == 2)
                    {
                        string previousOwner = targetRegion.owner;
                        mapManager.changeRegionOwner(launchedRegion.owner, targetRegion);
                        moveArmy(attackarmy, defenderregion, launchedRegion.owner);
                        List<Region> borderRegions = mapManager.borderRegions(targetRegion);
                        List<Region> regionsToChange = new List<Region>();
                        foreach (Region region in borderRegions)
                            if (region.owner == previousOwner)
                                regionsToChange.Add(region);
                        if (!regionsToChange.Any())
                        {
                            Console.WriteLine("Army anihilated");
                            armyManager.removeArmy(defencearmy, previousOwner, armyManager.getArmyByName(defencearmy).general.name);
                            armyManager.removeArmyMaintenance(defencearmy);
                        }
                        else
                        {
                            Random rnd = new Random();
                            int index = rnd.Next(0, regionsToChange.Count - 1);
                            moveArmy(defencearmy, regionsToChange[index].name, regionsToChange[index].owner);
                        }
                    }
                    else if (attackOutcome == 3) //Not sure if anything else should happen down here TBH, I will keep that in check
                    {

                    }
                    else if (attackOutcome == 4)
                    {

                    }
                    else if (attackOutcome == 5)
                    {

                    }
                    updateAllFactionsMaintenance();
                    return true;
                }
            }
            else
            {
                string previousOwner = targetRegion.owner;
                mapManager.changeRegionOwner(launchedRegion.owner, targetRegion);
                moveArmy(attackarmy, defenderregion, defenderregion);
                return true;
            }
        }

        public bool annexNeutralRegion(string army, string _targetRegion, string comingRegion)
        {
            Region launchedRegion = mapManager.searchRegionByName(comingRegion);
            Region targetRegion = mapManager.searchRegionByName(_targetRegion);

            if (targetRegion.owner == "Neutral")
            {
                mapManager.changeRegionOwner(launchedRegion.owner, targetRegion);
                moveArmy(army, _targetRegion, launchedRegion.owner);
                return true;
            }
            return false;
        }

        private List<int> getRegionResourceOutput(Region region)
        {
            List<int> resources = new List<int>();
            RegionType rType = mapManager.getTypeInformations(region.type);
            resources.Add(rType.gold); //0
            resources.Add(rType.wood); //1
            resources.Add(rType.food); //2
            resources.Add(rType.manpower); //3
            return resources;
        }

        private List<int> getFactionResourceOutput(Faction faction)
        {
            List<Region> factionRegions = mapManager.getFactionRegions(faction.name);

            if (factionRegions.Any())
            {
                List<int> resources = new List<int>();
                resources.Add(0); //0 gold
                resources.Add(0); //1 wood
                resources.Add(0); //2 food 
                resources.Add(0); //3 manpower
                foreach (Region region in factionRegions)
                {
                    List<int> regionResources = getRegionResourceOutput(region);
                    int i = 0;
                    foreach (int resource in regionResources)
                    {
                        resources[i] += resource;
                        i++;
                    }

                }
                return resources;
            }
            return null;
        }

        private bool updateFactionResourcesTurn(Faction faction)
        {
            int index = factionManager.factionByNameIndex(faction.name);
            List<int> outputs = getFactionResourceOutput(faction);
            if (outputs != null)
            {
                FactionMaintenance maintenance = factionMaintenances[index];
                int goldResult = outputs[0] - maintenance.gold;
                int woodResult = outputs[1] - maintenance.wood;
                int foodResult = outputs[2] - maintenance.food;
                int manpower = outputs[3];
                factionManager.updateFactionResources(faction.name, goldResult, manpower, woodResult, foodResult);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void updateFactionResources()
        {
            foreach (Faction faction in factionManager.getFactions())
            {
                bool worked = updateFactionResourcesTurn(faction);
            }
        }

        //Defeats and victory checks

        private void checkDefeats()
        {
            List<Region> regions = mapManager.getRegionsByType("Capital");
            List<Region> temp = mapManager.getRegionsByType("SpecialCapital");
            List<int> regionCounts = new List<int>();
            foreach (Faction faction in factionManager.getFactions())
            {
                regionCounts.Add(mapManager.getFactionRegions(faction.name).Count);
            }
            foreach (Region region in temp)
                regions.Add(region);
            defeatedcount += factionManager.checkFactionsDefeat(regions, defeatManager, regionCounts);
            Console.WriteLine("Defeated: " + defeatedcount);
        }

        public bool capitulation(string faction)
        {
            bool capitulate = factionManager.capitulation(faction);
            if (capitulate)
                defeatedcount++; 
            return capitulate; 
        }

        public Faction checkGameStatus(GameRoom room) //returns the winning faction
        {
            checkDefeats();
            if (defeatedcount == 3)
            {
                List<Faction> winner = factionManager.getActiveFactions();
                if (winner.Count == 1)
                    return winner[0];
            }
            else if (defeatedcount == 4)
            {
                ICollection<Faction> factions = factionManager.getDefeatedFactions();
                if (factions.Count == 4)
                {
                    List<RoomPlayer> defeated = room.getClassificationPlayers();
                    string winning = defeated[room.getClassificationPlayers().Count - 1].faction;
                    factionManager.factionByName(winning).defeated = false;
                    List<Faction> winner = factionManager.getActiveFactions();
                    if (winner.Count == 1)
                        return winner[0];
                }
            }

            return null;
        }

        //Turn Related Material

        public Timer turnSpanner(MessageHandlerService instance, Timer initTimer, GameRoom room)
        {
            if (turn == 0)
            {
                initTimer.Dispose();
                timer2 = enableActions();
                timer3 = executeActionsTimer(room);
            }
            TimeSpan startTimeSpan = TimeSpan.Zero;
            TimeSpan periodTimeSpan = TimeSpan.FromSeconds(60);
            Timer timer = new Timer(async (e) =>
            {

                if (turn != 0)
                {
                    editable = false;
                    GameRequest GameRequest = new GameRequest();
                    GameRequest.EventType = "UpdateInformation";
                    try
                    {
                        await instance.updateTurnAsync(GameRequest, room, "server");
                    } catch (HttpRequestException exc)
                    {
                        Console.WriteLine("Server could not parse user data;\n" + exc.ToString());
                    }
                    canMakeEditable = true;
                }
                Console.WriteLine("Editable? =>" + editable);
                turn++;
            }, null, startTimeSpan, periodTimeSpan);
            return timer;
        }

        private Timer enableActions()
        {
            TimeSpan startTimeSpan2 = TimeSpan.Zero;
            TimeSpan periodTimeSpan2 = TimeSpan.FromSeconds(15);
            Timer timer = new Timer((e) =>
            {
                if (turn != 0 && canMakeEditable)
                {
                    editable = true;
                    canMakeEditable = false;
                }

                Console.WriteLine(DateTime.UtcNow + " Timer2. Editable =>" + editable + "; Can Make Editable: " + canMakeEditable);
            }, null, startTimeSpan2, periodTimeSpan2);
            return timer;
        }

        private Timer executeActionsTimer(GameRoom room)
        {
            TimeSpan startTimeSpan3 = TimeSpan.Zero;
            TimeSpan periodTimeSpan3 = TimeSpan.FromSeconds(20);
            Timer timer = new Timer((e) =>
            {
                executeQueueActions(room);
                Console.WriteLine("Timer3");
            }, null, startTimeSpan3, periodTimeSpan3);
            return timer;
        }

        public void executeQueueActions(GameRoom room)
        {
            Console.WriteLine(this.getQueue().Count);
            while (this.getQueue().TryDequeue(out GameRequest request) && this.getPlayerQueue().TryDequeue(out string player))
            {
                Protocol.executeAction(request, room, player);
                Console.WriteLine(request.EventType + "; by: " + player);
            }
        }

        public Faction getFaction(string faction)
        {
            return factionManager.factionByName(faction);
        }

        //BACKEND LOGIC

        public List<IdFactionResponse> readPlayerFactions(GameRoom room)
        {
            List<IdFactionResponse> idFactions = new List<IdFactionResponse>();
            List<RoomPlayer> players = room.Players.PlayerList.ToList();
            foreach (RoomPlayer roomPlayer in players)
            {
                idFactions.Add(new IdFactionResponse(roomPlayer.playerId, roomPlayer.faction));
            }
            return idFactions;
        }

        public bool gameFinished(string faction, GameRoom room) //To really use this method which will handle the finished game
        {
            RoomPlayer rp = winner(faction, room);
            if (rp != null)
            {
                room.getClassificationPlayers().Add(rp);
                PlayerMatchdata matchdata = new PlayerMatchdata(room.roomId, rp.playerId, 1, rp.armiesCreated, rp.regionsConquered);
                room.insertPlayerData(matchdata);
                Console.WriteLine("Player to matchdata: " + matchdata.playerId);
                return true;
            }

            return false;
        }

        public RoomPlayer winner(string faction, GameRoom room)
        {
            IEnumerator roomPlayer = room.Players.PlayerList.GetEnumerator();
            while (roomPlayer.MoveNext())
            {
                RoomPlayer rp = (RoomPlayer)roomPlayer.Current;
                if (faction.Equals(rp.faction))
                    return rp;
            }
            return null;
        }

        public bool isFactionActive(RoomPlayer player, GameRoom room)
        {
            return getPlayerFaction(player, room).defeated == false;
        }

        public Faction getPlayerFaction(RoomPlayer player, GameRoom room)
        {
            return room.GameInstance.getFaction(player.faction);
        }

        public void removePlayerFromRoom(RoomPlayer player, GameRoom room)
        {
            room.Players.RemovePlayer(player.playerId);
        }

        public void removeDefeatedPlayers(GameRoom room)
        {

            IEnumerator roomPlayer = room.Players.PlayerList.GetEnumerator();
            while (roomPlayer.MoveNext())
            {
                RoomPlayer rp = (RoomPlayer)roomPlayer.Current;
                Console.WriteLine(1 + ";" + rp.faction);
                bool playerDataExists = false;
                foreach (PlayerMatchdata data in room.getMatchdata())
                    if (data.playerId.Equals(rp.playerId))
                        playerDataExists = true;
                if (!isFactionActive(rp, room) && !playerDataExists)
                {
                    Console.WriteLine(2 + ";" + rp.faction);
                    int placement = 4 - room.insertDefeatedPlayer(rp);
                    //removePlayerFromRoom(rp, room);    
                    rp.placement = placement;
                    PlayerMatchdata matchdata = new PlayerMatchdata(room.roomId, rp.playerId, rp.placement, rp.armiesCreated, rp.regionsConquered);
                    room.insertPlayerData(matchdata);
                    Console.WriteLine("Player to matchdata: " + matchdata.playerId);
                }
            }

        }

        //NEW QUEUE METHODS:

        public ConcurrentQueue<GameRequest> getQueue()
        {
            return concurrentGameQueue;
        }

        public ConcurrentQueue<string> getPlayerQueue()
        {
            return concurrentPlayerQueue;
        }

        public void closeTimers()
        {
            timer2.Dispose();
            timer3.Dispose();
            try
            {
                    turnTimer.DisposeAsync();        
            }
            catch(NullReferenceException exc)
            {
                Console.WriteLine("Timers were already down: " + exc.ToString());
            }
        }

    }
}
