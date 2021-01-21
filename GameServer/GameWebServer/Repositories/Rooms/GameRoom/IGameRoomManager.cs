using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Room;

namespace GameWebServer.Repositories
{
    public interface IGameRoomManager : IRoomManager
    {
        /// <summary>
        /// Creates a new room with all respective players.
        /// </summary>
        /// <param name="players">a collection of references to the players being added</param>
        /// <returns>the created Room's ID</returns>
        GameRoom CreateRoom(ICollection<RoomPlayer> players);

        /// <summary>
        /// Updates a player's faction. The faction name should be a valid one.
        /// </summary>
        /// <param name="roomId">the room identification</param>
        /// <param name="playerId">the existing player identification</param>
        /// <param name="faction">the new faction</param>
        void UpdatePlayerFaction(string roomId, string playerId, string faction);

        /// <summary>
        /// Updates a player's socket connection. The player should be in a room.
        /// <list type="bullet">
        /// <description>If the player already has an open connection, it is closed and the new one takes place.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="roomId">the room identification</param>
        /// <param name="playerId">the updating player identification</param>
        /// <param name="conn">the new socket connection reference</param>
        /// <returns>A Task object representing the async closure of the previous socket connection, if one exists, 
        /// as well as a collection containing the players present in the room</returns>
        Task<ICollection<RoomPlayer>> UpdatePlayerConnection(string roomId, string playerId, WebSocket conn);

        /// <summary>
        /// Removes a player's socket connection. The player should be in a room.
        /// <list type="bullet">
        /// <item>
        /// <description>The websocket is closed and its reference inside the room is updated to *void*.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="roomId">the room identification</param>
        /// <param name="playerId">the existing player identification</param>
        /// <returns>A Task object representing the async closure of the existing socket connection</returns>
        Task RemovePlayerConnection(string roomId, string playerId);

        /// <summary>
        /// Add Game Room created on matchmaking
        /// </summary>
        /// <param name="gameRoom">the new game room created on matchmaking</param>
        void AddMatchMakingGameRoom(GameRoom gameRoom);
    }
}