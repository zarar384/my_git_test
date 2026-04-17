namespace LeaveMeAloneCSharp.Utils.Adapters
{
    // Adapter: converts the APM-style asynchronous pattern (Begin/End) of LegacyCalculationService
    // into a Task-based asynchronous pattern (TAP).
    // Much simpler than EAP because .NET already provides FromAsync helper
    public static class LegacyCalculationServiceExtensions
    {
        public static Task<int> CalculateSquareAsync(this LegacyCalculationService service, int number)
        {
            return Task<int>.Factory.FromAsync(
                service.BeginCalculateSquare,
                service.EndCalculateSquare,
                number,
                null);
        }
    }
}
