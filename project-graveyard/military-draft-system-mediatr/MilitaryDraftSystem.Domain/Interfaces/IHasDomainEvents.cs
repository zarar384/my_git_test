namespace MilitaryDraftSystem.Domain.Interfaces
{
    /// <summary>
    /// Marks an entity as capable of raising and exposing domain events so that
    /// infrastructure (e.g. the EF Core save-changes interceptor) can publish
    /// them generically, without special-casing individual entity types.
    /// </summary>
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        void ClearDomainEvents();
    }
}
