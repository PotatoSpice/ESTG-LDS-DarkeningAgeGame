using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameWebAPI.Exceptions
{
    public class NoRecordedMatchesException : Exception
    {

        public NoRecordedMatchesException()
           : base($"No recorded matches were found") { }

        public NoRecordedMatchesException(string target)
            : base($"No recorded matches were found for '{target}'.") { }

    }
}
