using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Room;

namespace GameWebServer.Services
{
    public class MatchMakingHandlerService : IMatchMakingHandlerService
    {
        private ConcurrentQueue<Room> lobbysAddQueue;
        private ConcurrentDictionary<string, string> lobbysInQueue;
        private ConcurrentQueue<GameRoom> readyGameRooms;
        private ConcurrentQueue<ConcurrentQueue<Room>> playersTempRooms;
        private Thread queueThread;

        public MatchMakingHandlerService()
        {
            lobbysAddQueue = new ConcurrentQueue<Room>();
            readyGameRooms = new ConcurrentQueue<GameRoom>();
            playersTempRooms = new ConcurrentQueue<ConcurrentQueue<Room>>();
            lobbysInQueue = new ConcurrentDictionary<string, string>();

            queueThread = new Thread(GenerateGameRooms);
            queueThread.Start();
        }

        public void AddRoomToQueue(Room room)
        {
            lobbysAddQueue.Enqueue(room);
            lobbysInQueue.TryAdd(room.roomId, "");
        }

        private void GenerateGameRooms()
        {
            while (true)
            {
                Room room;
                if (lobbysAddQueue.TryDequeue(out room))
                {
                    //se existirem lobbys temporarios
                    if (playersTempRooms.Count != 0)
                    {
                        bool roomAdded = false;
                        ConcurrentQueue<Room>[] tempRooms = playersTempRooms.ToArray();

                        for (int i = 0; i < tempRooms.Length && !roomAdded; i++)
                        {
                            int count = TempRoomOccupation(tempRooms[i]);

                            //se a sala a contar com o novo nao exceder o limite de 4
                            if (count + room.Players.PlayerList.Count <= 4)
                            {
                                Room[] tempRoomArray = tempRooms[i].ToArray();
                                List<Room> tempRoomList = tempRoomArray.ToList();
                                playersTempRooms = new ConcurrentQueue<ConcurrentQueue<Room>>(playersTempRooms.Where(x => x != tempRooms[i]));
                                tempRoomList.Add(room);
                                ConcurrentQueue<Room> addedRoomToTemp = new ConcurrentQueue<Room>(tempRoomList);

                                roomAdded = true;

                                //se a sala ficar cheia
                                if (TempRoomOccupation(addedRoomToTemp) == 4)
                                {
                                    Room[] completeTempRoom = addedRoomToTemp.ToArray();
                                    GameRoom gameRoom = null;
                                    
                                    //se nenhum dessa sala cancelou
                                    if(CreateGameRoom(addedRoomToTemp, out gameRoom))
                                    {
                                        readyGameRooms.Enqueue(gameRoom);
                                    }
                                    else
                                    {
                                        playersTempRooms.Enqueue(addedRoomToTemp);
                                    }
                                }
                                else
                                {
                                    playersTempRooms.Enqueue(addedRoomToTemp);
                                }
                            }
                        }
                        //se em nehuma das salas tiver espaço
                        if (roomAdded == false)
                        {
                            ConcurrentQueue<Room> newRoom = new ConcurrentQueue<Room>();
                            newRoom.Enqueue(room);
                            playersTempRooms.Enqueue(newRoom);
                        }
                    }
                    else
                    {
                        ConcurrentQueue<Room> newRoom = new ConcurrentQueue<Room>();
                        newRoom.Enqueue(room);
                        playersTempRooms.Enqueue(newRoom);
                    }
                }
            }
        }

        public GameRoom GetGameRoom(Room room)
        {
            GameRoom gameRoom = null;
            bool roomCreated = false;
            bool cancelled = false;

            while (!roomCreated && !cancelled)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>(lobbysInQueue);

                //se cancelou
                if (!dict.TryGetValue(room.roomId, out string value))
                {
                    cancelled = true;
                }

                GameRoom[] games = readyGameRooms.ToArray();
                foreach (GameRoom gR in games)
                {

                    if (gR.Players.ContainsPlayer(room.Players.hostID))
                    {
                        roomCreated = true;
                        gameRoom = gR;
                    }
                }
                if (roomCreated)
                {
                    cancelled = false;
                }
            }

            return gameRoom;
        }

        public bool CanRemoveFromQueue(Room room)
        {
            GameRoom[] readyGames = readyGameRooms.ToArray();
            foreach(GameRoom gameRoom in readyGames)
            {
                if (gameRoom.Players.ContainsPlayer(room.Players.hostID))
                {
                    return false;
                }
            }
            return true;
        }

        public bool RemoveFromQueue(Room room)
        {
            if (CanRemoveFromQueue(room))
            {
                lobbysInQueue.TryRemove(room.roomId, out var ok);

                lobbysAddQueue = new ConcurrentQueue<Room>(lobbysAddQueue.Where(x => x != room));

                Room[] foundTempRoom = null;
                ConcurrentQueue<Room>[] tempRooms = playersTempRooms.ToArray();
                foreach(ConcurrentQueue<Room> tempRoom in tempRooms)
                {
                    Room[] tempRoomArray = tempRoom.ToArray();
                    foreach(Room r in tempRoomArray)
                    {
                        if(r.roomId == room.roomId)
                        {
                            playersTempRooms = new ConcurrentQueue<ConcurrentQueue<Room>>(playersTempRooms.Where(x => x != tempRoom));
                            foundTempRoom = tempRoomArray;
                        }
                    }
                }
                if (foundTempRoom != null)
                {
                    foreach (Room r in foundTempRoom)
                    {
                        if(r.roomId != room.roomId)
                        {
                            lobbysAddQueue.Enqueue(r);
                        }
                    }
                }

                return true;
            }
            return false;
        }

        public void DeleteGameRoom(string roomId, string gameRoomID)
        {
            int count = 0;
            Dictionary<string, string> dict = new Dictionary<string, string>(lobbysInQueue);
            foreach(var item in dict)
            {
                if (item.Key == roomId)
                {
                    lobbysInQueue.TryRemove(roomId, out var ok);
                }
            }
            dict = new Dictionary<string, string>(lobbysInQueue);
            foreach (var item in dict)
            {
                if (item.Value == gameRoomID)
                {
                    count += 1;
                }
            }

            if(count == 0)
            {
                readyGameRooms = new ConcurrentQueue<GameRoom>(readyGameRooms.Where(x => x.roomId != gameRoomID));
            }
        }
        
        private int TempRoomOccupation(ConcurrentQueue<Room> tempRoom)
        {
            int count = 0;

            Room[] tempRoomList = tempRoom.ToArray();
            foreach (Room r in tempRoomList)
            {
                count += r.Players.PlayerCount;
            }

            return count;
        }

        private bool CreateGameRoom(ConcurrentQueue<Room> readyRoom, out GameRoom gameRoom)
        {
            gameRoom = null;
            List<RoomPlayer> players = new List<RoomPlayer>();
            Dictionary<string, string> dict = new Dictionary<string, string>(lobbysInQueue);
            Room[] roomList = readyRoom.ToArray();

            foreach (Room r in roomList)
            {
                //se nao cancelou
                if(dict.TryGetValue(r.roomId, out string value))
                {
                    foreach (RoomPlayer player in r.Players.PlayerList)
                    {
                        players.Add(new RoomPlayer(player.playerId, null));
                    }
                }
                else
                {
                    return false;
                }
            }

            gameRoom = new GameRoom(players);

            foreach (Room r in readyRoom)
            {
                lobbysInQueue.TryUpdate(r.roomId, gameRoom.roomId, "");
            }
            return true;
        }
    }
}

