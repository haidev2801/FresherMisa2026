namespace FresherMisa2026.Application.Exceptions
{
    /// <summary>
    /// Trùng mã nhân viên ở tầng DB (unique index / duplicate key), sau khi map từ MySQL.
    /// </summary>
    public sealed class DuplicateEmployeeCodeException : Exception
    {
        public DuplicateEmployeeCodeException()
            : base("Mã nhân viên đã tồn tại")
        {
        }
    }
}
