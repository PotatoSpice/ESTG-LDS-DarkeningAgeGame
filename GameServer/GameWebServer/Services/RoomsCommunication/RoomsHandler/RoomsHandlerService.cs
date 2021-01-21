using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameWebServer.Exceptions;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Room;
using GameWebServer.Repositories;

namespace GameWebServer.Services
{
    public abstract class RoomsHandlerService : IRoomsHandlerService
    {
        protected IRoomManager _lobby_rooms { get; private set; }
        protected IGameRoomManager _ingame_rooms { get; private set; }

        public RoomsHandlerService(IRoomManager lobby_rooms, IGameRoomManager ingame_rooms)
        {
            _lobby_rooms = lobby_rooms;
            _ingame_rooms = ingame_rooms;
        }

        // This method should be called when a player isn't in a room and wants to create one
        // Note: the method returns an async task since it is meant to be overriden and may have to await tasks.
        public virtual async Task<string> OnConnectedHostLobby(RoomPlayer host_player, bool findMatch)
        {
            return _lobby_rooms.CreateRoom(host_player, findMatch);
        }

        // This method should be called when a player isn't in a room and wants to join one
        // Note: the method returns an async task since it is meant to be overriden and may have to await tasks.
        public virtual async Task<ICollection<string>> OnConnectedJoinLobby(string roomId, RoomPlayer player)
        {
            return _lobby_rooms.AddPlayerToRoom(roomId, player);
        }

        // This method should be called when a player disconnects, both from lobby or from the game itself
        public async Task OnPlayerLeft(string roomId, string playerId)
        {
            if (_ingame_rooms.HasRoom(roomId))
            {
                await OnPlayerLeftGame(roomId, playerId);
            }
            else if(_lobby_rooms.HasRoom(roomId))
            {
                await OnPlayerLeftLobby(roomId, playerId);
            }
        }

        // This method should be used when a player leaves a lobby manually.
        // Returns the new host if the player who disconnected was the host. Null otherwise and when the room becomes empty.
        protected virtual async Task<RoomPlayer> OnPlayerLeftLobby(string roomId, string playerId)
        {
            return await _lobby_rooms.DeletePlayerFromRoom(roomId, playerId);
        }

        // This method should be used when a player disconnects but should still be in the room.
        // An example is when a players exits the game. The player should continue *in-game* but in this case *away*.
        //  The player can still reconnect to the game using the method *UpdateConnection*.
        protected virtual async Task OnPlayerLeftGame(string roomId, string playerId)
        {
            await _ingame_rooms.RemovePlayerConnection(roomId, playerId);
        }

        public string CheckIfPlayerInGame(string playerId)
        {
            ICollection<Room> rooms = _ingame_rooms.GetAll();
            foreach(Room room in rooms)
                if (room.Players.ContainsPlayer(playerId))
                    return room.roomId;
            return null;
        }

        public bool CheckIfPlayerInStartedGame(string roomId, string playerId)
        {
            GameRoom room = _ingame_rooms.GetRoomById(roomId) as GameRoom;
            if (room.IsGameLoaded() && room.Players.GetPlayer(playerId).socket != null)
                return true;
            return false;
        }

        // This method should be used to update an existing player's connection. The player should be in a room.
        // Some example cases of this are when a player leaves the game and wants to enter again 
        //  or when the player switches from the GameClient (browser) to the GameInterface (Unity)
        public virtual async Task<ICollection<RoomPlayer>> UpdateConnection(string roomId, string playerId, WebSocket newConn)
        {
            if (_ingame_rooms.HasRoom(roomId))
            {
                GameRoom room = _ingame_rooms.GetRoomById(roomId) as GameRoom;
                if (room.IsGameLoaded())
                {
                    return await _ingame_rooms.UpdatePlayerConnection(roomId, playerId, newConn);
                } else {
                    throw new GameWarningException("The GameRoom hasn't started the game yet");
                }
            } else {
                throw new GameWarningException("GameRoom was not created or doesn't exist");
            }
        }

        public async Task SendMessageAsync(WebSocket socket, string sendData)
        {
            if(socket == null || socket.State != WebSocketState.Open) return;

            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.UTF8.GetBytes(sendData),
                                                                    offset: 0,
                                                                    count: Encoding.UTF8.GetBytes(sendData).Length),
                                    messageType: WebSocketMessageType.Text,
                                    endOfMessage: true,
                                    cancellationToken: CancellationToken.None);
        }

        public async Task SendToOthersInLobbyAsync(string roomId, string playerToIgnore, string sendData)
        {
            Room room = _lobby_rooms.GetRoomById(roomId);

            foreach(ConnectionPlayer player in room.Players.PlayerList)
            {
                if(player.playerId != playerToIgnore && player.socket != null && player.socket.State == WebSocketState.Open)
                    await SendMessageAsync(player.socket, sendData);
            }
        }

        public async Task SendToAllInLobbyAsync(string roomId, string sendData)
        {
            Room room = _lobby_rooms.GetRoomById(roomId);

            foreach(ConnectionPlayer player in room.Players.PlayerList)
            {
                if(player.socket != null && player.socket.State == WebSocketState.Open)
                    await SendMessageAsync(player.socket, sendData);
            }
        }

        public async Task SendToAllInGameAsync(string roomId, string sendData)
        {
            Room room = _ingame_rooms.GetRoomById(roomId);

            foreach(ConnectionPlayer player in room.Players.PlayerList)
            {
                if(player.socket != null && player.socket.State == WebSocketState.Open)
                    await SendMessageAsync(player.socket, sendData);
            }
        }

        // Method for handling all messages coming from a player that's in a GameClient's Lobby
        public abstract Task HandleLobbyAsync(string roomId, string playerId, WebSocket playerConn, string receivedData);

        // Method for handling all messages coming from a player that's in-game
        public abstract Task HandleGameAsync(string roomId, string playerId, WebSocket playerConn, string receivedData);
    }
}