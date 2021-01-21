using System;

namespace GameWebServer.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested player is not found inside any room memory.
    /// </summary>
    public class PlayerNotFoundException : ApplicationException
    {
        private string _message = string.Empty;
    
        public string ExceptionMessage { get { return _message; } set { _message = value; } }
    
        public PlayerNotFoundException() : base() { }
    
        public PlayerNotFoundException(string message) : base(message)
        {
            _message = message;
        }
    }   
}