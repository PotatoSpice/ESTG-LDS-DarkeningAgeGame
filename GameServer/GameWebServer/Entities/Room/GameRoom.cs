using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameWebServer.Entities.Game;
using GameWebServer.Entities.ExternalData;
using GameWebServer.Entities.Player;
using GameWebServer.Utils;

namespace GameWebServer.Entities.Room
{
    public class GameRoom : Room
    {
        private GameInstance _game;
        private Dictionary<string, bool> connectedPlayers; 
        public GameInstance GameInstance { get { return _game; } }
        public Timer initTimer {get; set;}
        public int connected { get; set; }
        public int eventcicles { get; set; }
        private List<RoomPlayer> classificationPlayers;
        private bool gameStarted;
        private List<PlayerMatchdata> matchdata;

        public GameRoom(ICollection<RoomPlayer> p_players) : base(p_players)
        {
            this._game = new GameInstance(roomId);
            connected = 0;
            eventcicles = 0;
            matchdata = new List<PlayerMatchdata>();
            classificationPlayers = new List<RoomPlayer>();
            connectedPlayers = loadPlayers(this.Players.PlayerList);
            this.AssignDefaultPlayerFactions();
            gameStarted = false;
        }

        public int connect()
        {
            return connected++; 
        }

        public int insertDefeatedPlayer(RoomPlayer player)
        {
            classificationPlayers.Add(player);
            return classificationPlayers.IndexOf(player); 
        }

        public void insertPlayerData(PlayerMatchdata data)
        {
            matchdata.Add(data); 
        }

        public List<PlayerMatchdata> getMatchdata()
        {
            return matchdata; 
        }

        public List<RoomPlayer> getClassificationPlayers()
        {
            return classificationPlayers; 
        }

        /// Randomizes faction attribution for all players inside a room.
        private void AssignDefaultPlayerFactions()
        {
            Random rng = new Random();
            List<int> order = new List<int>();
            for (int ix = 0; ix < 4; ix++)
                order.Add(ix);
            int nx = 4;
            while (nx > 1)
            {
                nx--;
                int k = rng.Next(nx + 1);
                int value = order[k];
                order[k] = order[nx];
                order[nx] = value;
            }

            List<Faction> factions = (List<Faction>)MapReader.readFactions();

            int i = 0;
            IEnumerator roomPlayer = this.Players.PlayerList.GetEnumerator();
            while (roomPlayer.MoveNext())
            {
                Console.WriteLine(i);
                RoomPlayer rp = (RoomPlayer)roomPlayer.Current;
                rp.faction = factions[order[i]].name;
                RoomPlayer newCopy = (RoomPlayer)roomPlayer.Current;
                Console.WriteLine(newCopy.playerId + "; " + newCopy.faction);
                i++;
            }
        }

            //Connection Dictionary

            public Dictionary<string, bool> getDictionary()
        {
            return connectedPlayers; 
        }

        public Dictionary<string, bool> loadPlayers(ICollection<RoomPlayer> players)
        {
            Dictionary<string, bool> TempConnectedPlayers = new Dictionary<string, bool>(); 
            foreach(RoomPlayer player in players)
                TempConnectedPlayers.Add(player.playerId, false);
            return TempConnectedPlayers; 
        }

        public bool allPlayersConnected()
        {
            int count = 0;
            foreach(var player in connectedPlayers)
                if (player.Value == true)
                    count++;         
            if (count != 4)
                return false;
            return true; 
        }

        public void connectedPlayer(string playerId)
        {
            connectedPlayers[playerId] = true;
        }

        public void disconnectedPlayer(string playerId)
        {
            connectedPlayers[playerId] = false;
        }

        public bool setGameStarted()
        {
            return gameStarted = true;
        }

        public int countConnectedPlayers()
        {
            int count = 0;
            foreach (var player in connectedPlayers)
                if (player.Value == true)
                    count++;
            return count; 
        }

        public bool IsGameStarted()
        {
            return gameStarted; 
        }

        public bool IsGameLoaded()
        {
            return this.GameInstance.IsGameLoaded(); 
        }

    }
}