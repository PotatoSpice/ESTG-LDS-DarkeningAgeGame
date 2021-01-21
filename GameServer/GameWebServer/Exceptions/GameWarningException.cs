using System;

namespace GameWebServer.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when something goes wrong with a Game Room.
    /// </summary>
    public class GameWarningException : ApplicationException
    {
        private string _message = string.Empty;
    
        public string ExceptionMessage { get { return _message; } set { _message = value; } }
    
        public GameWarningException() : base() { }
    
        public GameWarningException(string message) : base(message)
        {
            _message = message;
        }
    }   
}