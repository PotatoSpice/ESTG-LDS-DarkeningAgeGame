using GameWebServer.Entities.Room;

namespace GameWebServer.Services
{
    public interface IMatchMakingHandlerService
    {
        /// <summary>
        /// Adds lobby to matchmaking queue
        /// </summary>
        /// <param name="room"></param>lobby room
        void AddRoomToQueue(Room room);

        /// <summary>
        /// Wait for GameRoom containing the lobby
        /// </summary>
        /// <param name="room"></param>lobby room
        /// <returns>the GameRoom when the room is found or null if the lobby cancels matchmaking</returns>
        GameRoom GetGameRoom(Room room);

        /// <summary>
        /// Check if lobby is in any GameRoom created
        /// </summary>
        /// <param name="room"></param>lobby room
        /// <returns>true if the lobby isn't in any GameRoom created, otherwise returns false</returns>
        bool CanRemoveFromQueue(Room room);

        /// <summary>
        /// Remove lobby from matchmaking
        /// </summary>
        /// <param name="room"></param>lobby room
        /// <returns>true if can lobby was removed from matchmaking else returns false</returns>
        bool RemoveFromQueue(Room room);

        /// <summary>
        /// Delete GameRoom after all lobbys in it receive the GameObject instance
        /// </summary>
        /// <param name="roomId"></param>lobby room identification
        /// <param name="gameRoomID"></param>game room identification
        void DeleteGameRoom(string roomId, string gameRoomID);
    }
}