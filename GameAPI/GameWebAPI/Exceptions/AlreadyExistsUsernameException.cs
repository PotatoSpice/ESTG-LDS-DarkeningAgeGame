using System;

namespace GameWebAPI.Exceptions
{
    public class AlreadyExistsUsernameException : EntityAlreadyExistsException
    {
        public AlreadyExistsUsernameException() 
            : base() { }
        
        public AlreadyExistsUsernameException(string username) 
            : base($"Player with username '{username}' already exists in the database.") { }
    }
}