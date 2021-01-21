using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GameWebServer.Exceptions;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Game;
using GameWebServer.Entities.Room;
using GameWebServer.Models.Responses;
using GameWebServer.Repositories;
using GameWebServer.Utils;
using GameWebServer.Models.Requests;
using GameWebServer.Entities;
using GameWebServer.Entities.ExternalData;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace GameWebServer.Services
{
    public class MessageHandlerService : RoomsHandlerService, IMessageHandlerService
    {
        private IMatchMakingHandlerService _matchmaking;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpFactory; 
        public MessageHandlerService(IRoomManager lobby_rooms, IGameRoomManager game_rooms, IMatchMakingHandlerService matchmakingService, IConfiguration config, IHttpClientFactory httpFactory) : base(lobby_rooms, game_rooms)
        {
            _matchmaking = matchmakingService;
            _config = config;
            _httpFactory = httpFactory; 
        }

        public override async Task<string> OnConnectedHostLobby(RoomPlayer player, bool findMatch)
        {
            string new_room_id = await base.OnConnectedHostLobby(player, findMatch);

            await base.SendMessageAsync(player.socket, JsonConvert.SerializeObject(
                    new LobbyAlertResponse("new-room-id", new_room_id)));
            return new_room_id;
        }

        public override async Task<ICollection<string>> OnConnectedJoinLobby(string roomId, RoomPlayer player)
        {
            ICollection<string> players = await base.OnConnectedJoinLobby(roomId, player);
            
            await base.SendToOthersInLobbyAsync(roomId, player.playerId, JsonConvert.SerializeObject(
                new LobbyAlertResponse("player-connected", player.playerId)));
            
            await base.SendMessageAsync(player.socket, JsonConvert.SerializeObject(
                new LobbyAlertExtendedResponse("lobby-players", players)));
            return players;
        }

        protected override async Task<RoomPlayer> OnPlayerLeftLobby(string roomId, string playerId)
        {
            RoomPlayer new_host = await base.OnPlayerLeftLobby(roomId, playerId);

            if (new_host != null) {
                await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                    new LobbyAlertResponse("player-disconnected", playerId)));
                if (new_host.playerId != playerId) {
                    await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                        new LobbyAlertResponse("new-host", new_host.playerId)));
                }
            }
            return new_host;
        }

        public override async Task HandleLobbyAsync(string roomId, string playerId, WebSocket playerConn, string receivedData)
        {
            Room lobby = null;
            GameRoom gameRoom = null;
            Task find_matchmaking = null;
            LobbyRequest request = JsonConvert.DeserializeObject<LobbyRequest>(receivedData);
            switch (request.eventType)
            {
                case "chat-message":
                    if (!String.IsNullOrWhiteSpace(request.data)) {
                        await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                            new LobbyChatResponse(playerId, request.data, DateTime.Now.ToShortTimeString())));
                    }
                    break;
                case "cancel-matchmaking":
                    await CancelMatchmaking(roomId);
                    break;
                case "start-matchmaking":
                    lobby = base._lobby_rooms.GetRoomById(roomId);
                    if (lobby.Players.hostID != playerId)
                    {
                        throw new LobbyWarningException("Only the room's host can start matchmaking");
                    }
                    else if (!lobby.Players.matchmakingRoom) 
                    {
                        throw new LobbyWarningException("Invalid room for matchmaking. 1-2 players are allowed.");
                    }
                    else
                    {
                        if (find_matchmaking != null)
                        {
                            throw new LobbyWarningException("Room already started matchmaking");
                        }
                        else
                        {
                            _matchmaking.AddRoomToQueue(lobby);
                            // notify other players in the lobby
                            await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                                new LobbyAlertResponse("finding-match", "true")));
                            find_matchmaking = Task.Run(
                            async () =>
                            {
                                Console.WriteLine("Start search");
                                await Task.Run(() => gameRoom = _matchmaking.GetGameRoom(lobby));
                                if (gameRoom != null)
                                {
                                    Console.WriteLine("Found");

                                    base._ingame_rooms.AddMatchMakingGameRoom(gameRoom);
                                    await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                                        new LobbyAlertResponse("game-started", gameRoom.roomId)));

                                    _matchmaking.DeleteGameRoom(lobby.roomId, gameRoom.roomId);
                                    await gameRoom.GameInstance.loadGameInfo();
                                }
                            });
                        }
                    }
                    break;
                case "start-game":
                    lobby = base._lobby_rooms.GetRoomById(roomId);
                    if (lobby.Players.hostID != playerId)
                    {
                        throw new LobbyWarningException("Only the room's host can start the game");
                    }
                    else if (lobby.Players.PlayerCount < 4) 
                    {
                        throw new LobbyWarningException("Room should be full before starting the game");
                    }
                    else
                    {
                        GameRoom room = base._ingame_rooms.CreateRoom(lobby.Players.PlayerList);

                        await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                                new LobbyAlertResponse("game-started", room.roomId)));
                        await room.GameInstance.loadGameInfo();
                    }
                    break;
                default:
                    throw new LobbyWarningException("Invalid Request Type");
            }
        }

        public override async Task<ICollection<RoomPlayer>> UpdateConnection(string roomId, string playerId, WebSocket newConn)
        {
            ICollection<RoomPlayer> players = await base.UpdateConnection(roomId, playerId, newConn);
            List<RoomPlayerDTO> temp = new List<RoomPlayerDTO>();
            foreach (RoomPlayer elem in players)
            {
                temp.Add(new RoomPlayerDTO(elem.playerId, elem.faction));
            }

            await base.SendMessageAsync(newConn, 
                JsonConvert.SerializeObject(new GameUpdatedConnResponse(temp)));
            return players;
        }

        public override async Task HandleGameAsync(string roomId, string playerId, WebSocket playerConn, string receivedData)
        {
            GameRoom room = base._ingame_rooms.GetRoomById(roomId) as GameRoom;
            GameRequest requestModel = Protocol.requestParser(receivedData);
            RoomPlayer player = room.Players.GetPlayer(playerId);


            if (requestModel.EventType == "ChatMessage")
            {
                var message = "{\"EventType\":\"ChatMessage\",\"Username\":\"" + requestModel.data[0] + "\", \"Message\": \"" + requestModel.data[1] + "\"}";
                await base.SendToAllInGameAsync(roomId, message);
            }
            else if (room.GameInstance.isFactionActive(player, room))
            {
                if (requestModel.EventType == "UpdateInformation" && room.IsGameStarted())
                {
                    
                    room.GameInstance.executeQueueActions(room);

                    GameRequest updateResources = new GameRequest();
                    updateResources.EventType = "UpdateResources";
                    Protocol.executeAction(updateResources, room, playerId);
                    await messageSender(requestModel, room);
                }
                if (requestModel.EventType == "InitialInformation")
                {    
                    if (room.countConnectedPlayers() < 4 && room.countConnectedPlayers() > 0)
                    {
                        room.connectedPlayer(playerId);
                    }
                    else 
                    {
                        initialGameCheck(requestModel, room);
                        room.connectedPlayer(playerId);
                    }

                    if (room.allPlayersConnected() && !room.IsGameStarted())
                    {
                        await sendInitialInformation(requestModel, room);
                        room.setGameStarted(); 
                        room.GameInstance.turnSpanner(this, room.initTimer, room);
                    } else if (room.IsGameStarted())
                    {
                        await messageSenderIndividual(requestModel, room, playerConn); 
                    }
                    // if(room.GameInstance.playerCount==1)
                    //await sendInitialInformation(requestModel, room);
                }
                else if (requestModel.EventType == "PlayerFactions")
                {
                    await messageSender(requestModel, room);
                }
                else if (room.GameInstance.editable)
                {
                    Console.WriteLine(room.GameInstance.editable);
                    Console.WriteLine("In here: " + requestModel.EventType); 
                    room.GameInstance.getQueue().Enqueue(requestModel);
                    room.GameInstance.getPlayerQueue().Enqueue(playerId); 
                }
                else if (requestModel.EventType == "Connected")
                { //    DEPRECATED BUT I'LL KEEP IT
                  //int connected = room.connect();
                  //if (connected == 4)
                  //{
                  //   room.initTimer.Dispose();
                  //  room.GameInstance.turnSpanner(this, room.initTimer);
                  //}
                }
            }
        }

        private async Task messageSender(GameRequest requestModel, GameRoom room)
        {
            List<IdFactionResponse> idFactions = room.GameInstance.readPlayerFactions(room);
            var message = Protocol.sendRequests(requestModel, room.GameInstance, idFactions);
            Console.WriteLine(message);
            await base.SendToAllInGameAsync(room.roomId, message);
        }

        private async Task messageSenderIndividual(GameRequest request, GameRoom room, WebSocket playerConn)
        {
            List<IdFactionResponse> idFactions = room.GameInstance.readPlayerFactions(room);
            var message = Protocol.sendRequests(request, room.GameInstance, idFactions);
            Console.WriteLine(message);
            await base.SendMessageAsync(playerConn, message);
        }

        private void initialGameCheck(GameRequest request, GameRoom room)
        {
                room.GameInstance.setGameLoaded();
                room.initTimer = countDownGameStart(request, room);
        }

        private async Task sendInitialInformation(GameRequest request, GameRoom room)
        {
            await messageSender(request, room);
        }

        private System.Threading.Timer countDownGameStart(GameRequest request, GameRoom room)
        {     
            TimeSpan startTimeSpan = TimeSpan.Zero;
            TimeSpan periodTimeSpan = TimeSpan.FromSeconds(60);

            System.Threading.Timer timer = new System.Threading.Timer(async (e) =>
            {
                Console.WriteLine(DateTime.Now);

                if (room.eventcicles == 1)
                {
                    Console.WriteLine("HERE IS COUNTDOWN " + room.eventcicles);
                    await sendInitialInformation(request, room);
                    room.setGameStarted();
                    room.GameInstance.turnTimer = room.GameInstance.turnSpanner(this, room.initTimer, room);
                }
                room.eventcicles++;
            }, null, startTimeSpan, periodTimeSpan);

            return timer; 
        }

        public async Task updateTurnAsync(GameRequest requestModel, GameRoom room, string playerID)
        {
          
         
            room.GameInstance.executeQueueActions(room);
            Console.WriteLine(DateTime.Now);
            GameRequest updateResources = new GameRequest();
            updateResources.EventType = "UpdateResources";
            Faction faction = room.GameInstance.checkGameStatus(room); //check if there is a winner or not;
            bool gamefinish = false;
            room.GameInstance.removeDefeatedPlayers(room); //removes defeat players. If there is none, there is no problem
            if (faction != null) //if there is a winner
            {
                gamefinish = room.GameInstance.gameFinished(faction.name, room); //mark game as finished and handle somethings
            }

            if (gamefinish == false) // if game is not finished, what should happen
            {
                Protocol.executeAction(updateResources, room, playerID);
                await messageSender(requestModel, room);
            }
            else
            {
              
                foreach (PlayerMatchdata pmd in room.getMatchdata())
                {
                    string playerData = Utils.JsonConverter<PlayerMatchdata>.convertObjectToJsonString(pmd);
                    Console.WriteLine(playerData);
                    await HandlePlayerGameData(_httpFactory, playerData); 
                }
                //How to handle the game finished state when sending info to the client
                Protocol.executeAction(updateResources, room, playerID);
                await messageSender(requestModel, room);
                room.GameInstance.closeTimers(); 
                await _ingame_rooms.CloseRoom(room.roomId); 
            }
        }

        public async Task CancelMatchmaking(string roomId)
        {
            Room lobby = base._lobby_rooms.GetRoomById(roomId);

            _matchmaking.RemoveFromQueue(lobby);
            // notify other players in the lobby
            await base.SendToAllInLobbyAsync(roomId, JsonConvert.SerializeObject(
                new LobbyAlertResponse("finding-match", "false")));
        }

        private async Task HandlePlayerGameData(IHttpClientFactory httpFactory, string playerJsonData)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, _config["AppSettings:ServerMatchdata"]);
            request.Content = new StringContent(playerJsonData, Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", _config["AppSettings:GameServerKey"]);
            using (HttpClient client = httpFactory.CreateClient())
            {
                using var httpResponse = await client.SendAsync(request);
                httpResponse.EnsureSuccessStatusCode();
            }          
        }

    }
}
