using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Room;
using GameWebServer.Exceptions;

namespace GameWebServer.Repositories
{
    public class RoomManager : IRoomManager
    {
        protected ConcurrentDictionary<string, Room> _rooms;
        private Semaphore _pool;

        public RoomManager()
        {
            _rooms = new ConcurrentDictionary<string, Room>();
            _pool = new Semaphore(1, 1);
        }

        /// <summary>
        /// Closes a socket's connection asynchronously. Allows only one thread at a time, using a Semaphore.
        /// </summary>
        /// <param name="socket">the socket to be disconnected</param>
        /// <returns>A Task object representing the async task</returns>
        protected async Task CloseSocketConn(WebSocket socket)
        {
            if (socket == null) return;
            _pool.WaitOne();
            if (socket.State == WebSocketState.Open) 
            {
                await socket.CloseOutputAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                        statusDescription: "Connection closed by manager ...",
                                        cancellationToken: CancellationToken.None);
            }
            _pool.Release(1);
        }

        public bool HasRoom(string roomId)
        {
            return _rooms.ContainsKey(roomId);
        }

        public Room GetRoomById(string roomId)
        {
            Room room;
            if (!_rooms.TryGetValue(roomId, out room)) 
            {
                throw new RoomNotFoundException($"'{roomId}' not found.");
            }
            return room;
        }

        public bool HasPlayerInRoom(string roomId, string playerId)
        {
            Room room;
            if (_rooms.TryGetValue(roomId, out room))
            {
                return room.Players.ContainsPlayer(playerId);
            } else {
                Console.WriteLine($"Room '{roomId}' not found.");
                return false;
            }
        }

        public RoomPlayer GetPlayerInRoom(string roomId, string playerId, out Room out_room)
        {
            Room room = GetRoomById(roomId);
            RoomPlayer player = room.Players.GetPlayer(playerId);
            if (player != null)
            {
                out_room = room;
                return player;
            } 
            else {
                throw new PlayerNotFoundException($"'{playerId}' not found for room '{roomId}'.");
            }
        }

        public string GetRoomIdFromPlayer(string playerId)
        {
            foreach(Room room in _rooms.Values)
            {
                if (room.Players.ContainsPlayer(playerId)) return room.roomId;
            }
            Console.WriteLine($"[RoomManager] Room not found for player '{playerId}'.");
            return null;
        }

        public string GetRoomIdFromConn(WebSocket conn)
        {
            foreach(Room room in _rooms.Values)
            {
                if (room.Players.ContainsPlayerConn(conn)) return room.roomId;
            }
            Console.WriteLine($"[RoomManager] Room not found for socket connection!.");
            return null;
        }

        public ICollection<Room> GetAll()
        {
            return _rooms.Values;
        }

        // Creates a new room with a respective "host" (note: a room must have at least one player)
        // Returns the created Room's ID.
        public string CreateRoom(RoomPlayer playerHost, bool findMatch)
        {
            if (!string.IsNullOrWhiteSpace(GetRoomIdFromPlayer(playerHost.playerId)))
            {
                throw new AlreadyInLobbyException($"'{playerHost.playerId}' is already in a room.");
            }
            Room new_room = new Room(playerHost, findMatch);
            _rooms.TryAdd(new_room.roomId, new_room);
            Console.WriteLine($"\n[RoomManager] room '{new_room.roomId}' created with host '{new_room.Players.hostID}'.");
            return new_room.roomId;
        }
        
        // Adds a player to an existing room
        public ICollection<string> AddPlayerToRoom(string roomId, RoomPlayer player)
        {
            Room oldR = GetRoomById(roomId);
            if (oldR.Players.ContainsPlayer(player.playerId))
            {
                throw new AlreadyInLobbyException($"'{player.playerId}' is already in a room.");
            }
            Room newR = oldR;
            if (newR.Players.AddPlayer(player))
            {
                _rooms.TryUpdate(roomId, newR, oldR);
                Console.WriteLine($"\n[RoomManager] player '{player.playerId}' added to room '{roomId}'.");
                ICollection<RoomPlayer> playersInRoom = oldR.Players.PlayerList;
                List<string> list = new List<string>();
                foreach (RoomPlayer p in playersInRoom) 
                {
                    list.Add(p.playerId);
                }
                return list;
            } else {
                throw new FullRoomException("Maximum 2/4 players allowed in a room, for matchmaking and custom game respectively!");
            }
        }

        // Searches and Removes a player from a room and terminates its socket connection. 
        // - If the player was a host, a new room host player is elected and returned.
        // - If the player is the last in the room, the room is deleted.
        public async Task<RoomPlayer> DeletePlayerFromRoom(string roomId, string playerId)
        {
            Room room = GetRoomById(roomId);
            RoomPlayer player;
            if (room.Players.RemovePlayer(playerId, out player))
            {
                Console.WriteLine($"\n[RoomManager] player '{playerId}' deleted from room '{roomId}'.");
                await CloseSocketConn(player.socket);
                if (room.Players.PlayerCount != 0)
                {
                    return room.Players.GetPlayer(room.Players.hostID);
                }
                else // delete the room if it becomes empty
                {
                    DeleteRoom(roomId);
                    return null;
                }
            } else {
                Console.WriteLine($"\n[RoomManager] Player '{playerId}' already removed from room '{roomId}'!");
                return null;
            }
        }

        // Closes all players socket connections in a room.
        // Notes: 
        //  - when a connection is closed the player related to it is removed from the room automatically.
        //  - Similarly, a room is automatically deleted when there are no players left inside.
        public async Task CloseRoom(string roomId)
        {
            Room room = GetRoomById(roomId);
            foreach(RoomPlayer player in room.Players.PlayerList)
            {
                await CloseSocketConn(player.socket);
            }
            DeleteRoom(roomId);
        }

        private Room DeleteRoom(string roomId)
        {
            Room room;
            if (_rooms.TryRemove(roomId, out room))
                Console.WriteLine($"\n[RoomManager] room deleted '{roomId}'.");
            return room;
        }
    }
}