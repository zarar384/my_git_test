using MediatR;
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

        public RunPopulationGenerationCommandHandler(
            IAppDbContext db,
            IMediator mediator,
            IPopulationGenerationService populationGenerationService)
        {
            _db = db;
            _mediator = mediator;
            _populationGenerationService = populationGenerationService;
        }

        public async Task Handle(
            RunPopulationGenerationCommand request,
            CancellationToken cancellationToken)
        {
            // Load generator configuration.
            var generator = await _db.GetPopulationGenerator(cancellationToken);

            // Stop execution if the generator is disabled.
            if (generator is null || !generator.Enabled)
            {
                return;
            }

            // Generate a new population.
            var citizens = _populationGenerationService.Generate(generator);

            // Register generated citizens for persistence.
            foreach (var citizen in citizens)
            {
                _db.AddCitizen(citizen);
            }

            // Update generation timestamp.
            generator.MarkExecuted(DateTimeOffset.UtcNow);

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
        }
    }
}
