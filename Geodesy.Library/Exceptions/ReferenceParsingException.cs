using System;

namespace Geodesy.Library.Exceptions
{
    public class ReferenceParsingException : Exception
    {
        private const string ERROR_MESSAGE_BASE = "Invalid {0} string ({1}) - {2}";

        public ReferenceParsingException() { }

        public ReferenceParsingException(Type type, string reference, string details) 
            : base(string.Format(ERROR_MESSAGE_BASE, type.Name, reference, details)) { }
    }
}
