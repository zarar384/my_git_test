using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MilitaryDraftSystem.Application.Common.Mappings;
using MilitaryDraftSystem.Domain.Interfaces;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Interceptors
{
    public class DomainEventsInterceptor: SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        public DomainEventsInterceptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, 
            int result, 
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if(context == null)
                return result;

            // Get every tracked entity (of any type) that has pending domain events.
            var entitiesWithEvents = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);

            foreach (var entity in entitiesWithEvents)
            {
                foreach(var domainEvent in entity.DomainEvents)
                {
                    // map to MediatR event 
                    var notification = DomainEventMapper.Map(domainEvent);

                    // publish the event
                    await _mediator.Publish(notification, cancellationToken);
                }

                // clear domain events
                // Prevent publishing the same events multiple times.
                entity.ClearDomainEvents();
            }

            return result;
        }
    }
}
