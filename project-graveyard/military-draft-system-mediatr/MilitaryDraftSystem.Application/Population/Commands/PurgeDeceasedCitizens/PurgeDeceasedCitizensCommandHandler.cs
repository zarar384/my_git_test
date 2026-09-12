using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Configuration;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Population.Commands.PurgeDeceasedCitizens
{
    /// <summary>
    /// Physically deletes deceased citizens from the main population table once
    /// their retention window has elapsed. Their cemetery record and death
    /// statistics were already recorded when they died, so deleting the
    /// Citizen row here never loses any historical information.
    /// </summary>
    public sealed class PurgeDeceasedCitizensCommandHandler
        : IRequestHandler<PurgeDeceasedCitizensCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<PurgeDeceasedCitizensCommandHandler> _logger;

        public PurgeDeceasedCitizensCommandHandler(
            IAppDbContext db,
            ILogger<PurgeDeceasedCitizensCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(
            PurgeDeceasedCitizensCommand request,
            CancellationToken cancellationToken)
        {
            var cutoff = DateTimeOffset.UtcNow - SimulationRetention.DeceasedCitizenRetention;

            var citizensToPurge = await _db.GetDeceasedCitizensOlderThan(cutoff, cancellationToken);

            if (citizensToPurge.Count == 0)
            {
                return;
            }

            foreach (var citizen in citizensToPurge)
            {
                var cemeteryRecord = await _db.GetCemeteryRecord(SubjectType.Citizen, citizen.Id, cancellationToken);
                cemeteryRecord?.MarkOriginalRecordDeleted();

                _db.RemoveCitizen(citizen);
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Purged {Count} deceased citizen(s) from the population table.",
                citizensToPurge.Count);
        }
    }
}
