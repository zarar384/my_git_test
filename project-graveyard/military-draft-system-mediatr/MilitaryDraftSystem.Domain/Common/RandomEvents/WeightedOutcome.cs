namespace MilitaryDraftSystem.Domain.Common.RandomEvents
{
    /// <summary>
    /// Represents a single possible outcome within a weighted random event catalog.
    /// New outcomes can be added to a catalog without changing any selection logic,
    /// keeping the random event model extensible.
    /// </summary>
    public sealed record WeightedOutcome<TResult>(
        string Id,
        string Name,
        double Weight,
        TResult Result);
}
