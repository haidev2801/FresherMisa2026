using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities.Exceptions
{
    public class DatabaseException : Exception
    {
        public string? ErrorCode { get; }

        public DatabaseException(string message, string? errorCode = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
