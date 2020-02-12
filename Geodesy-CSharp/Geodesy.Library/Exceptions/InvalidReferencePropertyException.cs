using Geodesy.Library.Enums;
using System;

namespace Geodesy.Library.Exceptions
{
    public class InvalidReferencePropertyException : Exception
    {
        private const string ERROR_MESSAGE_BASE = "Invalid {0} property : {1} - {2}";

        public InvalidReferencePropertyException() { }

        public InvalidReferencePropertyException(Type type, UtmEnum paramater, string reference)
            : base(string.Format(ERROR_MESSAGE_BASE, type.Name, paramater.ToString(), reference)) { }
    }
}
