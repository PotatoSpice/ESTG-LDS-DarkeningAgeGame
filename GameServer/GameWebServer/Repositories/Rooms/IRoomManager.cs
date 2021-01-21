using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using GameWebServer.Entities.Player;
using GameWebServer.Entities.Room;

namespace GameWebServer.Repositories
{
    public interface IRoomManager
    {
        bool HasRoom(string roomId);

        Room GetRoomById(string roomId);

        bool HasPlayerInRoom(string roomId, string playerId);

        RoomPlayer GetPlayerInRoom(string roomId, string playerId, out Room out_room);

        string GetRoomIdFromPlayer(string playerId);

        string GetRoomIdFromConn(WebSocket conn);

        ICollection<Room> GetAll();

        /// <summary>
        /// Creates a new room with a respective "host" (note: a room must have at least one player)
        /// </summary>
        /// <param name="findMatch">tells if the room is made for matchmaking or a custom game</param>
        /// <returns>the created Room's ID</returns>
        string CreateRoom(RoomPlayer playerHost, bool findMatch);

        /// <summary>
        /// Adds a player to an existing room. Only 4 players are allowed at maximum.
        /// </summary>
        /// <param name="roomId">the room identification</param>
        /// <param name="player">a reference to the player being saved</param>
        /// <returns>a collection containing the players already present in the room</returns>
        ICollection<string> AddPlayerToRoom(string roomId, RoomPlayer player);
        
        /// <summary>
        /// Searches and Removes a player from a room and terminates its socket connection.
        /// <list type="bullet">
        /// <item>
        /// <description>If the player was a host, a new room host player is elected and returned.</description>
        /// </item>
        /// <item>
        /// <description>If the player is the last in the room, the room is deleted.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="roomId">The room identification.</param>
        /// <param name="playerId">The existing player identification.</param>
        /// <returns>A Task object representing the async closure of the socket, with a reference to the new Hosting player.</returns>
        Task<RoomPlayer> DeletePlayerFromRoom(string roomId, string playerId);

        /// <summary>
        /// Closes all players socket connections in a room.
        /// <list type="bullet">
        /// <item>
        /// <description>When a connection is closed the player related to it is removed from the room automatically.</description>
        /// </item>
        /// <item>
        /// <description>Similarly, a room is automatically deleted when there are no players left inside.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="roomId">The room identification.</param>
        /// <returns>A Task object representing the async closure of the players socket connections.</returns>
        Task CloseRoom(string roomId);
    }
}