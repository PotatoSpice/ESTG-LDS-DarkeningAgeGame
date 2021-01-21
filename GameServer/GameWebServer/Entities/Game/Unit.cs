namespace GameWebServer.Entities.Game
{
    public class Unit
    {
        public string name { get; set; } //will serve as the type
        // FOR NOW THIS WON'T APPLY :public string faction { get; set; }
        public int maxManpower { get; set; }
        public int maxMovementSpeed { get; set; }
        public float attackEarly { get; set; }
        public float attackMid { get; set; }
        public float attackLate { get; set; }
        public float defenceEarly { get; set; }
        public float defenceMid { get; set; }
        public float defenceLate { get; set; }
        public int goldCost { get; set; }
        public int woodCost { get; set; }
        public int foodCost { get; set; }
        public int foodMaintenance { get; set; }
        public int goldMaintenance { get; set; }


    }
}
