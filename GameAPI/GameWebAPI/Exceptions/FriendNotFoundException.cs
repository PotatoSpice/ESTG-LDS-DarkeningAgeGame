using System;

namespace GameWebAPI.Exceptions
{
    public class FriendNotFoundException : EntityNotFoundException
    {
        public FriendNotFoundException() : base("Players are not friends.")
        { }

        public FriendNotFoundException(string user, string targetUser) : base($"'{targetUser}' isn't friend of '{user}'.")
        { }
    }
}
