using System;

namespace GameWebAPI.Exceptions
{
    public class AlreadyFriendsException : EntityAlreadyExistsException
    {
        public AlreadyFriendsException()
            : base($"Players are already friends.") { }

        public AlreadyFriendsException(string user, string targetUser)
            : base($"Player with username '{user}' and '{targetUser}' already are friends.") { }
    }
}
