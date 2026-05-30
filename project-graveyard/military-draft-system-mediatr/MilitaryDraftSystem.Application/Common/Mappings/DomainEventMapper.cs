using MediatR;
using MilitaryDraftSystem.Application.Draft.Events;
using MilitaryDraftSystem.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MilitaryDraftSystem.Application.Common.Mappings
{
    public class DomainEventMapper
    {
        // map domain events to MediatR notifications
        public static INotification Map(object domainEvent)
        {
            return domainEvent switch
            {
                // map each domain event type to a corresponding MediatR notification type
              SummonsSentDomainEvent e => new SummonsSentEvent(e.CitizenId, e.SummonsId),

                _ => throw new ArgumentException($"No mapping defined for domain event type {domainEvent.GetType().Name}")
            };
        }
    }
}
