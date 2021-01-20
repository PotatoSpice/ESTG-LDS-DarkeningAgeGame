using System;

namespace GameWebAPI.Exceptions
{
    public class AlreadyExistsEmailException : EntityAlreadyExistsException
    {
        public AlreadyExistsEmailException() 
            : base($"Player already exists in the database with the given email.") { }

        public AlreadyExistsEmailException(string email) 
            : base($"Player with email '{email}' already exists in the database.") { }
    }
}