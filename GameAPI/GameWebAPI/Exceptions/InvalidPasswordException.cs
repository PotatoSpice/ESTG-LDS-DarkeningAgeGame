using System;

namespace GameWebAPI.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException() 
            : base($"Provided password is invalid.") { }

        public InvalidPasswordException(string target) 
            : base($"Failed to reset password for '{target}'.") { }
    }
}