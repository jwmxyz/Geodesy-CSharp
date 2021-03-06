﻿using Geodesy.Library.Enums;
using System;

namespace Geodesy.Library.Exceptions
{
    public class InvalidReferencePropertyException<T> : Exception
    {
        private const string ERROR_MESSAGE_BASE = "Invalid {0} property : {1} - {2}";

        public InvalidReferencePropertyException() { }

        public InvalidReferencePropertyException(Type type, T paramater, string reference)
            : base(string.Format(ERROR_MESSAGE_BASE, type.Name, paramater.ToString(), reference)) { }
    }
}
