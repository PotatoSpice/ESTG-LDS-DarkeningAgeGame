using System;
using System.Collections.Generic;
using GameWebServer.Entities.Game;

namespace GameWebServer.Models.Responses
{
    public class InitInformationResponse
    {

        public InitInformationResponse(int _turn, List<Region> _regions, List<RegionType> _regionTypes, List<Unit> _units, List<General> _generals, List<Faction> _factions, 
            List<FactionMaintenance> _maintenances, List<IdFactionResponse> _IdFactions, List<FactionsAtWar> _wars, List<DefeatConditions> _defeatConditions)
        {
            turn = _turn; 
            EventType = "InitialInformation"; 
            regions = _regions;
            regionTypes = _regionTypes;
            units = _units;
            generals = _generals; 
            factions = _factions;
            factionMaintenances = _maintenances;
            idFactions = _IdFactions;
            wars = _wars;
            defeatConditions = _defeatConditions; 
        }

        public int turn { get; set; }

        public String EventType;
        public List<Region> regions { get; set; }
        public List<RegionType> regionTypes { get; set; }
        public List<Unit> units { get; set; }
        public List<General> generals { get; set; }
        public List<Faction> factions { get; set; }
        public List<FactionMaintenance> factionMaintenances { get; set; }
        public List<IdFactionResponse> idFactions { get; set; }
        public List<FactionsAtWar> wars { get; set; }

        public List<DefeatConditions> defeatConditions { get; set; }

        //In next sprints, more information shall be sent via here
    }
}
