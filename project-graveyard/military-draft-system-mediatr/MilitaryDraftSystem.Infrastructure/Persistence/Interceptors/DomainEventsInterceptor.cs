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

            // Drain pending domain events in batches until none remain.
            //
            // Notification handlers for one event may themselves raise new
            // domain events (e.g. ApplyGuilt on a RecruitmentOfficer raising
            // OfficerCareerEndedDomainEvent) and/or call SaveChangesAsync
            // again on the same context (e.g. ApplyOfficerGuiltHandler,
            // RecordCitizenDeathHistoryHandler). That nested SaveChangesAsync
            // re-enters this very method.
            //
            // Each batch is removed from the change tracker (ClearDomainEvents)
            // *before* any of its events are published. This is what makes
            // re-entrancy safe: by the time a handler triggers a nested
            // SaveChangesAsync, the events currently being published have
            // already been cleared from their entities, so the nested call's
            // own snapshot can never see - and therefore never re-publish -
            // them. The nested call's loop only ever finds events that are
            // genuinely new, and it fully drains those before returning, so
            // nothing raised during handling is silently lost either.
            while (true)
            {
                var entitiesWithEvents = context.ChangeTracker
                    .Entries<IHasDomainEvents>()
                    .Where(x => x.Entity.DomainEvents.Count > 0)
                    .Select(x => x.Entity)
                    .ToList();

                if (entitiesWithEvents.Count == 0)
                    break;

                var pendingEvents = new List<Domain.Interfaces.IDomainEvent>();

                foreach (var entity in entitiesWithEvents)
                {
                    pendingEvents.AddRange(entity.DomainEvents);

                    // Clear immediately, before publishing, so a nested
                    // SaveChangesAsync triggered by a handler below can never
                    // observe (and re-dispatch) these same events.
                    entity.ClearDomainEvents();
                }

                foreach (var domainEvent in pendingEvents)
                {
                    var notification = DomainEventMapper.Map(domainEvent);

                    await _mediator.Publish(notification, cancellationToken);
                }
            }

            return result;
        }
    }
}

