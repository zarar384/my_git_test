using MediatR;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Draft.Commands.RunAutomaticRecruitment
{
    /// <summary>
    /// Handles the automatic recruitment process.
    /// </summary>
    public class RunAutomaticRecruitmentCommandHandler : IRequestHandler<RunAutomaticRecruitmentCommand>
    {
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;

        public RunAutomaticRecruitmentCommandHandler(
            IAppDbContext db,
            IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        public async Task Handle(RunAutomaticRecruitmentCommand request, CancellationToken cancellationToken)
        {
            // Load the automatic recruitment agent.
            var agent = await _db.GetAutomaticRecruitmentAgent(cancellationToken);

            // Stop processing if the agent is missing or disabled.
            if (agent is null || !agent.Enabled)
            {
                return;
            }

            // Load citizens eligible for automatic recruitment.
            var citizens = await _db.GetCitizensEligibleForAutomaticDraft(cancellationToken);

            foreach (var citizen in citizens)
            {
                // Execute domain logic and create a summons.
                var summons = citizen.Draft(
                    DraftSource.AutomaticAgent,
                    null,
                    agent.Id,
                    DateTime.UtcNow);

                // Schedule the summons for persistence.
                _db.AddSummons(summons);
            }

            // Update the last execution timestamp.
            agent.MarkExecuted(DateTime.UtcNow);

            // Persist all changes.
            await _db.SaveChangesAsync(cancellationToken);

            // Publish domain events raised during the draft process.
            foreach (var citizen in citizens)
            {
                foreach (var domainEvent in citizen.DomainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                // Prevent publishing the same events multiple times.
                citizen.ClearDomainEvents();
            }
        }
    }
}