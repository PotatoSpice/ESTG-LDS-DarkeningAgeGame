using System;

namespace GameWebServer.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to add a player to a filled room.
    /// </summary>
    public class FullRoomException : ApplicationException
    {
        private string _message = string.Empty;
    
        public string ExceptionMessage { get { return _message; } set { _message = value; } }
    
        public FullRoomException() : base() { }
    
        public FullRoomException(string message) : base(message)
        {
            _message = message;
        }
    }   
}