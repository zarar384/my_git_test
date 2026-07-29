using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Domain.Common
{
    /// <summary>
    /// Base class for all domain entities.
    /// Provides an identifier and domain event support.
    /// </summary>
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; } = default!;

        private readonly List<IDomainEvent> _domainEvents = [];

        // Exposes domain events without allowing external modification.
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        // Clears published domain events to prevent duplicate processing.
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
