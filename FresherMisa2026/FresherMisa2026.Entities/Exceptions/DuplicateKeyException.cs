namespace FresherMisa2026.Entities.Exceptions
{
    /// <summary>
    /// Exception ném ra khi vi phạm unique constraint ở tầng database.
    /// Thường xảy ra khi 2 request cùng lúc insert bản ghi có giá trị trùng nhau
    /// dù đã pass validate ở tầng service (race condition).
    /// </summary>
    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException(string message) : base(message)
        {
        }
    }
}
