namespace MilitaryDraftSystem.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when a requested entity (citizen, recruitment officer, player,
    /// etc.) does not exist. Distinct from <see cref="InvalidOperationException"/>,
    /// which is used for valid requests rejected by the current state of an
    /// existing entity (lifecycle conflicts, business rule violations).
    /// Mapped by the global HTTP exception handler to 404 Not Found.
    /// </summary>
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
