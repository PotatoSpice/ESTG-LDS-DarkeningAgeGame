using System;

namespace GameWebAPI.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException() : base("Entity not found in Db.")
        { }

        public EntityNotFoundException(string name) : base($"'{name}' not found.")
        { }
    }
}