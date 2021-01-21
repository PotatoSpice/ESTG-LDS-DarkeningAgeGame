namespace GameWebServer.Entities.Game
{
    public class ArmyMaintenance
    {

        public ArmyMaintenance(string _army)
        {
            army = _army;
            goldMaintenance = 0;
            foodMaintenance = 0; 
        }

        public string army { get; set; }
        public int goldMaintenance { get; set; }
        public int foodMaintenance { get; set; }

    }
}
