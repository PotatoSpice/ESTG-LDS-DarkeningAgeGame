namespace GameWebServer.Entities.Game
{
    public class FactionMaintenance
    {

        public FactionMaintenance(string _faction)
        {
            faction = _faction;
            food = 0;
            gold = 0;
            wood = 0; 
        }

        public string faction { get; set; }
        public int food { get; set; }
        public int gold { get; set; }
        public int wood { get; set; }

    }
}
