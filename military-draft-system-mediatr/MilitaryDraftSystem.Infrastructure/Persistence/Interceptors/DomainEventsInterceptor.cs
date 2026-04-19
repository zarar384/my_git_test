using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MilitaryDraftSystem.Application.Common.Mappings;
using MilitaryDraftSystem.Domain.Entities;

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

            // get all entities with domain events
            var entities = context.ChangeTracker
                .Entries<Citizen>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);

            foreach (var entity in entities)
            {
                foreach(var domainEvent in entity.DomainEvents)
                {
                    // map to MediatR event 
                    var notification = DomainEventMapper.Map(domainEvent);

                    // publish the event
                    await _mediator.Publish(notification, cancellationToken);
                }

                // clear domain events
                entity.DomainEvents.Clear();
            }

            return result;
        }
    }
}
