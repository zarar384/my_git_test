using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Draft.Commands.DraftCitizen
{
    /// <summary>
    /// Handles the manual drafting of a citizen by a recruitment officer.
    /// Unlike automatic recruitment, this decision is made by a human and
    /// therefore carries personal responsibility for its consequences.
    /// </summary>
    public sealed class DraftCitizenCommandHandler : IRequestHandler<DraftCitizenCommand>
    {
        private readonly IAppDbContext _db;
        private readonly IWorldNarrator _narrator;
        private readonly ILogger<DraftCitizenCommandHandler> _logger;

        public DraftCitizenCommandHandler(
            IAppDbContext db,
            IWorldNarrator narrator,
            ILogger<DraftCitizenCommandHandler> logger)
        {
            _db = db;
            _narrator = narrator;
            _logger = logger;
        }

        public async Task Handle(DraftCitizenCommand request, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            if (!officer.IsActive)
                throw new InvalidOperationException($"Recruitment officer {officer.Id} is not active ({officer.Status}) and cannot draft citizens.");

            var citizen = await _db.GetCitizen(request.CitizenId, cancellationToken)
                ?? throw new InvalidOperationException($"Citizen {request.CitizenId} was not found.");

            // Execute domain logic and create a summons. Citizen.Draft enforces eligibility.
            var summons = citizen.Draft(
                DraftSource.RecruitmentOfficer,
                officer.Id,
                null,
                DateTime.UtcNow);

            // The officer personally bears responsibility for this decision.
            officer.RegisterDraftedCitizen();

            _logger.LogInformation(
                "Citizen {CitizenId} was drafted by recruitment officer {OfficerId}.",
                citizen.Id,
                officer.Id);

            _narrator.CitizenDrafted(citizen, officer.Id, null);

            // Delivery is instantaneous in this simulation.
            summons.MarkDelivered();

            // Schedule the summons for persistence.
            _db.AddSummons(summons);

            _narrator.SummonsCreated(summons);

            await _db.SaveChangesAsync(cancellationToken);

            // Domain events raised above are published automatically by
            // DomainEventsInterceptor as part of SaveChangesAsync.
        }
    }
}
