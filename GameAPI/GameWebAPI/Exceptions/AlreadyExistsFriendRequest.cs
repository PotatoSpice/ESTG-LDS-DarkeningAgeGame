using System;

namespace GameWebAPI.Exceptions
{
    public class AlreadyExistsFriendRequest : EntityAlreadyExistsException
    {
        public AlreadyExistsFriendRequest()
            : base($"Already exists one friend request pendent between the two users.") { }

        public AlreadyExistsFriendRequest(string user, string targetUser)
            : base($"Player '{user}' already send  friend request to '{targetUser}'.") { }
    }
}
