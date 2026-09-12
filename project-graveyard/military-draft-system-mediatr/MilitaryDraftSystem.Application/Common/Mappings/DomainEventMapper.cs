using MediatR;
using MilitaryDraftSystem.Application.Draft.Events;
using MilitaryDraftSystem.Application.Population.Events;
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
                SummonsCreatedDomainEvent e => new SummonsSentEvent(e.CitizenId, e.SummonsId),
                CitizenDraftedDomainEvent e => new CitizenDraftedEvent(e.CitizenId),
                CitizenBecameAdultDomainEvent e => new CitizenBecameAdultEvent(e.CitizenId),
                CitizenDiedDomainEvent e => new CitizenDiedEvent(e.CitizenId, e.Death),
                OfficerDiedDomainEvent e => new OfficerDiedEvent(e.OfficerId, e.Death, e.Reason),
                OfficerCareerEndedDomainEvent e => new OfficerCareerEndedEvent(e.OfficerId, e.Reason),

                _ => throw new ArgumentException($"No mapping defined for domain event type {domainEvent.GetType().Name}")
            };
        }
    }
}
