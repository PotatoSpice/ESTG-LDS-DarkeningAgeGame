namespace GameWebServer.Entities.Game
{
    public class General
    {

        public General()
        {
            available = true; 
        }
        public bool available { get; set; }
        public string name { get; set; }
        public string faction { get; set; }
        public float bonus { get; set; }
        public int goldCost { get; set; }

    }
}
