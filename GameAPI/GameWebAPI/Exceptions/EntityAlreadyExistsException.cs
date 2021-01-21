using System;

namespace GameWebAPI.Exceptions
{
    public class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException() 
            : base($"Given Entity already exists in the database.") { }

        public EntityAlreadyExistsException(string parameter) 
            : base(parameter) { }
    }
}