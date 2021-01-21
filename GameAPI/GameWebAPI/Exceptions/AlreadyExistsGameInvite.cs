using System;

namespace GameWebAPI.Exceptions
{
    public class AlreadyExistsGameInvite : EntityAlreadyExistsException
    {
        public AlreadyExistsGameInvite()
            : base($"Already send game invite to that player.") { }

        public AlreadyExistsGameInvite(string user, string targetUser)
            : base($"Player '{user}' already send  game invite to '{targetUser}'.") { }
    }
}