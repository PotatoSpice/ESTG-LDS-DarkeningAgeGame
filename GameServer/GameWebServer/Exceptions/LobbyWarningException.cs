using System;

namespace GameWebServer.Exceptions
{
    /// <summary>
    /// Represents an exception that occurs when the user inputs something wrong, while inside a lobby.
    /// </summary>
    public class LobbyWarningException : ApplicationException
    {
        private string _message = string.Empty;
    
        public string ExceptionMessage { get { return _message; } set { _message = value; } }
    
        public LobbyWarningException() : base() { }
    
        public LobbyWarningException(string message) : base(message)
        {
            _message = message;
        }
    }
}