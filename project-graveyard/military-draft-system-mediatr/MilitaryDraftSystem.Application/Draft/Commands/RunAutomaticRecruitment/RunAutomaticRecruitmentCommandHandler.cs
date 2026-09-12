using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IWorldNarrator _narrator;
        private readonly ILogger<RunAutomaticRecruitmentCommandHandler> _logger;

        public RunAutomaticRecruitmentCommandHandler(
            IAppDbContext db,
            IWorldNarrator narrator,
            ILogger<RunAutomaticRecruitmentCommandHandler> logger)
        {
            _db = db;
            _narrator = narrator;
            _logger = logger;
        }

        public async Task Handle(RunAutomaticRecruitmentCommand request, CancellationToken cancellationToken)
        {
            // Load every agent currently allowed to act. Agents work independently.
            var agents = await _db.GetEnabledAutomaticRecruitmentAgents(cancellationToken);

            if (agents.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Automatic recruitment started with {AgentCount} active agent(s).", agents.Count);

            // Load citizens eligible for automatic recruitment.
            var citizens = await _db.GetCitizensEligibleForAutomaticDraft(cancellationToken);

            var draftedCitizens = new List<Domain.Entities.Citizen>();

            for (var i = 0; i < citizens.Count; i++)
            {
                var citizen = citizens[i];

                // Distribute eligible citizens across the active agents.
                var agent = agents[i % agents.Count];

                // Execute domain logic and create a summons.
                var summons = citizen.Draft(
                    DraftSource.AutomaticAgent,
                    null,
                    agent.Id,
                    DateTime.UtcNow);

                _logger.LogInformation(
                    "Citizen {CitizenId} was drafted by agent {AgentId}.",
                    citizen.Id,
                    agent.Id);

                _narrator.CitizenDrafted(citizen, null, agent.Id);

                // Delivery is instantaneous in this simulation.
                summons.MarkDelivered();

                // Schedule the summons for persistence.
                _db.AddSummons(summons);

                _narrator.SummonsCreated(summons);

                draftedCitizens.Add(citizen);
            }

            foreach (var agent in agents)
            {
                // Update the last execution timestamp.
                agent.MarkExecuted(DateTime.UtcNow);
            }

            // Persist all changes. Domain events raised above are published
            // automatically by DomainEventsInterceptor as part of this call.
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Automatic recruitment finished. {DraftedCount} citizen(s) drafted.",
                draftedCitizens.Count);
        }
    }
}