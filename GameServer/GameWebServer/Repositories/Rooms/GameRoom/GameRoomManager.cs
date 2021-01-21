using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Room;

namespace GameWebServer.Repositories
{
    public class GameRoomManager : RoomManager, IGameRoomManager
    {
        // Creates a new room with all respective players. Returns the created Room's ID.
        public GameRoom CreateRoom(ICollection<RoomPlayer> players)
        {
            if (players != null && players.Count > 0)
            {
                // "Clean" all the players connections from the list
                List<RoomPlayer> new_list = new List<RoomPlayer>();
                foreach(RoomPlayer player in players)
                {
                    new_list.Add(new RoomPlayer(player.playerId, null));
                }
                GameRoom new_room = new GameRoom(new_list);
                _rooms.TryAdd(new_room.roomId, new_room);
                Console.WriteLine("\n[GameRoomManager] game-room created: " + new_room.roomId);
                return new_room;
            }
            return null;
        }

        public void UpdatePlayerFaction(string roomId, string playerId, string faction)
        {
            // - verificar se o nome da faction é correto 
            //  (fazer um enum ou verificar por uma lista dos nomes possíveis das faction)
            // - verificar se a faction já está atribuida a outro player
            //  (se estiver, podemos "trocar" e o outro player passa a conter a faction do player atualizado)
            //  (não deverá ser possível dois players estarem em duas factions iguais)
        }

        // Updates a player's socket connection. The player should be in a room. 
        // If the player already has an open connection, it is closed and the new one takes place.
        public async Task<ICollection<RoomPlayer>> UpdatePlayerConnection(string roomId, string playerId, WebSocket conn)
        {
            Room oldR;
            RoomPlayer player = base.GetPlayerInRoom(roomId, playerId, out oldR);
            ICollection<RoomPlayer> beforeUpdate = oldR.Players.PlayerList;
            List<RoomPlayer> list = new List<RoomPlayer>();
            foreach (RoomPlayer p in beforeUpdate) 
            {
                list.Add(p);
            }
            Room newR = oldR;
            WebSocket oldS = player.socket;
            player.socket = conn;
            newR.Players.UpdatePlayer(player);

            if (base._rooms.TryUpdate(roomId, newR, oldR))
            { 
                Console.WriteLine($"\n[GameRoomManager] updated player connection: {playerId}, for room: {roomId}");
                await CloseSocketConn(oldS);
                return list;
            } else {
                Console.WriteLine($"\n[RoomManager] Player '{playerId}' already removed from room '{roomId}'!");
                return null;
            }
        }

        // Removes a player's socket connection. The player should be in a room.
        // The websocket is closed and its reference inside the room is updated to *void*.
        public async Task RemovePlayerConnection(string roomId, string playerId)
        {
            Room oldR;
            RoomPlayer player = base.GetPlayerInRoom(roomId, playerId, out oldR);
            Room newR = oldR;
            WebSocket oldS = player.socket;
            player.socket = null;
            newR.Players.UpdatePlayer(player);

            if (base._rooms.TryUpdate(roomId, newR, oldR))
            { 
                Console.WriteLine("\n[GameRoomManager] removed player connection: " + playerId);
                await CloseSocketConn(oldS);
            } else {
                Console.WriteLine($"\n[GameRoomManager] Player '{playerId}' already removed from room '{roomId}'!");
            }
        }

        // Add matchmaking GameRoom to _rooms if the room wasn't added already
        public void AddMatchMakingGameRoom(GameRoom gameRoom)
        {
            if (!_rooms.ContainsKey(gameRoom.roomId))
            {
                _rooms.TryAdd(gameRoom.roomId, gameRoom);
            }
        }
    }
}