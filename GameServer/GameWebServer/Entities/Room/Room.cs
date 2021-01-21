using System;
using System.Collections;
using System.Collections.Generic;
using GameWebServer.Entities.Player;

namespace GameWebServer.Entities.Room
{
    public class Room
    {
        public string roomId { get; private set; }
        private PlayersManager _players;

        public PlayersManager Players { get { return _players; } }
        public Room(RoomPlayer p_host, bool findMatch)
        {
            this.roomId = Guid.NewGuid().ToString();
            this._players = new PlayersManager(p_host, findMatch);
        }

        public Room(ICollection<RoomPlayer> p_players)
        {
            this.roomId = Guid.NewGuid().ToString();
            this._players = new PlayersManager(p_players);
        }

        public RoomPlayer getPlayerByID(string ID)
        {
            IEnumerator roomPlayer = Players.PlayerList.GetEnumerator();
            while (roomPlayer.MoveNext())
            {
                RoomPlayer rp = (RoomPlayer)roomPlayer.Current;
                if (rp.playerId.Equals(ID))
                {
                    return rp;  
                }
            }
            return null; 
        }

    }
}