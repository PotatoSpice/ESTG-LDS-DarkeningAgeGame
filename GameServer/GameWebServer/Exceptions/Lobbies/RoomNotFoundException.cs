using System;

namespace GameWebServer.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested room is not found in memory.
    /// </summary>
    public class RoomNotFoundException : ApplicationException
    {
        private string _message = string.Empty;
    
        public string ExceptionMessage { get { return _message; } set { _message = value; } }
    
        public RoomNotFoundException() : base() { }
    
        public RoomNotFoundException(string message) : base(message)
        {
            _message = message;
        }
    }   
}