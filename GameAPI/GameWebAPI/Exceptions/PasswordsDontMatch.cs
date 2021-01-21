using System;

namespace GameWebAPI.Exceptions
{
    public class PasswordsDontMatch : Exception
    {
        public PasswordsDontMatch() 
            : base($"Provided password dont match with actual password.") { }
    }
}