using System;
using System.Collections.Generic;
using GameWebServer.Entities.Game;

namespace GameWebServer.Models.Responses
{
    public class UpdatedInformationResponse
    {
        public UpdatedInformationResponse(int _turn, List<Region> _regions, List<Army> _armies, List<General> _generals, List<Faction> _factions, List<ArmyMaintenance> _armiesMaintenance, 
            List<FactionMaintenance> _factionMaintenances, List<FactionsAtWar> _wars)
        {
            turn = _turn; 
            EventType = "UpdateInformation";
            regions = _regions;
            armies = _armies;
            armiesMaintenance = _armiesMaintenance;
            generals = _generals;
            factionMaintenances = _factionMaintenances;
            factions = _factions;
            wars = _wars; 
        }

        public int turn { get; set; }

        public String EventType;

        public List<Region> regions { get; set; }

        public List<Army> armies { get; set; }

        public List<ArmyMaintenance> armiesMaintenance { get; set; }
        
        public List<General> generals { get; set; }

        public List<FactionMaintenance> factionMaintenances { get; set; }
        public List<Faction> factions { get; set; }

        public List<FactionsAtWar> wars { get; set; }

    }
}
