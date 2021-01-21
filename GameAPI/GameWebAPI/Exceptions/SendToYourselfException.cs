using System;

namespace GameWebAPI.Exceptions
{
    public class SendToYourselfException : Exception
    {
        public SendToYourselfException() 
            : base($"Can't send requests to yourself.") { }
    }
}