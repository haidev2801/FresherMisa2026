using System;

namespace FresherMisa2026.Infrastructure.Exceptions
{
    public class DuplicateKeyException : Exception
    {
        public string? Key { get; }

        public DuplicateKeyException(string? key, string message) : base(message)
        {
            Key = key;
        }
    }
}
