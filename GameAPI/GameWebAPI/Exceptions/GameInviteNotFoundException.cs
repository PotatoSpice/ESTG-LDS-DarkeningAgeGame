using System;

namespace GameWebAPI.Exceptions
{
    public class GameInviteNotFoundException : EntityNotFoundException
    {
        public GameInviteNotFoundException() : base("Game invite not found.")
        { }
    }
}