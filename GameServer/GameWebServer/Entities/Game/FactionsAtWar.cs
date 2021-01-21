namespace GameWebServer.Entities.Game
{
    public class FactionsAtWar
    {

        public FactionsAtWar(string _faction1, string _faction2)
        {
            faction1 = _faction1;
            faction2 = _faction2; 
            active = false;
        }

        public string faction1 { get; set; }
        public string faction2 { get; set; }
        public bool active { get; set; }       

    }
}
