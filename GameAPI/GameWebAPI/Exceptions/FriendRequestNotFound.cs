using System;

namespace GameWebAPI.Exceptions
{
    public class FriendRequestNotFound : EntityNotFoundException
    {
        public FriendRequestNotFound()
            : base($"Entity not found in Db.") { }
    }
}