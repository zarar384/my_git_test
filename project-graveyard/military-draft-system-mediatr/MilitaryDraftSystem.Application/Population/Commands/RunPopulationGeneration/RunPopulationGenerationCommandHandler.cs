using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Services.Interfaces;

namespace MilitaryDraftSystem.Application.Population.Commands.RunPopulationGeneration
{
    /// <summary>
    /// Handles automatic population generation.
    /// </summary>
    public sealed class RunPopulationGenerationCommandHandler
        : IRequestHandler<RunPopulationGenerationCommand>
    {
        private readonly IAppDbContext _db;
        private readonly IMediator _mediator;
        private readonly IPopulationGenerationService _populationGenerationService;
        private readonly IWorldNarrator _narrator;
        private readonly ILogger<RunPopulationGenerationCommandHandler> _logger;

        public RunPopulationGenerationCommandHandler(
            IAppDbContext db,
            IMediator mediator,
            IPopulationGenerationService populationGenerationService,
            IWorldNarrator narrator,
            ILogger<RunPopulationGenerationCommandHandler> logger)
        {
            _db = db;
            _mediator = mediator;
            _populationGenerationService = populationGenerationService;
            _narrator = narrator;
            _logger = logger;
        }

        public async Task Handle(
            RunPopulationGenerationCommand request,
            CancellationToken cancellationToken)
        {
            // Load God, the sole creator of new citizens.
            var god = await _db.GetGod(cancellationToken);

            // Stop execution if God is not currently creating life.
            if (god is null || !god.Enabled)
            {
                return;
            }

            _logger.LogInformation("Population generation started.");

            // Generate a new population.
            var citizens = _populationGenerationService.Generate(god);

            // Register generated citizens for persistence.
            foreach (var citizen in citizens)
            {
                _db.AddCitizen(citizen);

                _logger.LogInformation("Citizen {CitizenId} was created.", citizen.Id);
                _narrator.CitizenBorn(citizen);
            }

            // Update generation timestamp.
            god.MarkExecuted(DateTimeOffset.UtcNow);

            // Persist generated citizens.
            await _db.SaveChangesAsync(cancellationToken);

            // Publish generated domain events.
            foreach (var citizen in citizens)
            {
                foreach (var domainEvent in citizen.DomainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                citizen.ClearDomainEvents();
            }

            _logger.LogInformation(
                "Population generation finished. {CitizenCount} citizen(s) created.",
                citizens.Count);
        }
    }
}
