using MediatR;
using MilitaryDraftSystem.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MilitaryDraftSystem.Application.Draft.Commands.SendSummons
{
    public class SendSummonsHandler : IRequestHandler<SendSummonsCommand>
    {
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;

        public SendSummonsHandler(IAppDbContext db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        public async Task Handle(SendSummonsCommand request, CancellationToken cancellationToken)
        {
            // Load citizen with summons
            var citizen = await _db.GetCitizenWithSummons(request.CitizenId, cancellationToken);

            // Execute domain logic
            citizen?.SendSummons(request.Date);

            // Save changes
            await _db.SaveChangesAsync(cancellationToken);

            // Publish domain events
            if (citizen is not null)
            {
                foreach (var domainEvent in citizen.DomainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }

            // Clear domain events
            citizen?.DomainEvents?.Clear();
        }
    }
}
