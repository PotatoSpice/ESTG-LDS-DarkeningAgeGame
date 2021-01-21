namespace GameWebServer.Entities.Game
{
    public class Faction
    {

        public Faction()
        {
            defeated = false; 
            armyCount = 0;
            armistice = false; 
        }

        public string name { get; set; }
        public string affinity { get; set; }
        public string capital { get; set; }
        public bool defeated { get; set; }
        public int manpower { get; set; }
        public int wood { get; set; }
        public int gold { get; set; }
        public int food { get; set; }
        public int armyCount { get; set; }
        public bool armistice { get; set; }
        public int defeatCondition { get; set; }
    }
}
