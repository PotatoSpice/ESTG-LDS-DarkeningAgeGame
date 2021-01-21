using System;

namespace GameWebServer.Exceptions
{
    /// <summary>
    /// Exception thrown when a player tries to Host or Join a lobby while already present in one.
    /// </summary>
    public class AlreadyInLobbyException : ApplicationException
    {
        private string _message = string.Empty;
    
        public string ExceptionMessage { get { return _message; } set { _message = value; } }
    
        public AlreadyInLobbyException() : base() { }
    
        public AlreadyInLobbyException(string message) : base(message)
        {
            _message = message;
        }
    }   
}