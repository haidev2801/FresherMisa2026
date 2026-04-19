using System;
using System.Collections.Generic;
using System.Text;

namespace FresherMisa2026.Entities
{
    public class DuplicateException : Exception
    {
        public string Field { get; }

        public DuplicateException(string field, string message) : base(message)
        {
            Field = field;
        }
    }
}
