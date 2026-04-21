namespace FresherMisa2026.Application.Exceptions
{
    public class DuplicateDataException : Exception
    {
        public DuplicateDataException(string message) : base(message)
        {
        }

        public DuplicateDataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
