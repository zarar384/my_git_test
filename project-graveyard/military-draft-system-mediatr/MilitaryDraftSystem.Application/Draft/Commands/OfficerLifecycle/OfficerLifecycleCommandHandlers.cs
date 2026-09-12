using MediatR;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Application.Draft.Commands.OfficerLifecycle
{
    public sealed class GoOnLeaveCommandHandler : IRequestHandler<GoOnLeaveCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<GoOnLeaveCommandHandler> _logger;

        public GoOnLeaveCommandHandler(IAppDbContext db, ILogger<GoOnLeaveCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(GoOnLeaveCommand request, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            officer.GoOnLeave(DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recruitment officer {OfficerId} went on leave.", officer.Id);
        }
    }

    public sealed class ReturnFromLeaveCommandHandler : IRequestHandler<ReturnFromLeaveCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<ReturnFromLeaveCommandHandler> _logger;

        public ReturnFromLeaveCommandHandler(IAppDbContext db, ILogger<ReturnFromLeaveCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(ReturnFromLeaveCommand request, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            officer.ReturnFromLeave(DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recruitment officer {OfficerId} returned from leave.", officer.Id);
        }
    }

    public sealed class ResignCommandHandler : IRequestHandler<ResignCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<ResignCommandHandler> _logger;

        public ResignCommandHandler(IAppDbContext db, ILogger<ResignCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(ResignCommand request, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            officer.Resign(DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recruitment officer {OfficerId} resigned.", officer.Id);
        }
    }

    public sealed class RetireCommandHandler : IRequestHandler<RetireCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<RetireCommandHandler> _logger;

        public RetireCommandHandler(IAppDbContext db, ILogger<RetireCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(RetireCommand request, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            officer.RetireVoluntarily(DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recruitment officer {OfficerId} retired.", officer.Id);
        }
    }

    public sealed class FireOfficerCommandHandler : IRequestHandler<FireOfficerCommand>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<FireOfficerCommandHandler> _logger;

        public FireOfficerCommandHandler(IAppDbContext db, ILogger<FireOfficerCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Handle(FireOfficerCommand request, CancellationToken cancellationToken)
        {
            var officer = await _db.GetRecruitmentOfficer(request.RecruitmentOfficerId, cancellationToken)
                ?? throw new InvalidOperationException($"Recruitment officer {request.RecruitmentOfficerId} was not found.");

            officer.Fire(DateTimeOffset.UtcNow);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recruitment officer {OfficerId} was fired.", officer.Id);
        }
    }

    public sealed class StartOfficerCareerCommandHandler : IRequestHandler<StartOfficerCareerCommand, Guid>
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<StartOfficerCareerCommandHandler> _logger;

        public StartOfficerCareerCommandHandler(IAppDbContext db, ILogger<StartOfficerCareerCommandHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Guid> Handle(StartOfficerCareerCommand request, CancellationToken cancellationToken)
        {
            var player = await _db.GetPlayer(request.PlayerId, cancellationToken)
                ?? throw new InvalidOperationException($"Player {request.PlayerId} was not found.");

            var existingActiveOfficer = await _db.GetActiveRecruitmentOfficerByPlayer(player.Id, cancellationToken);

            if (existingActiveOfficer is not null)
            {
                throw new InvalidOperationException(
                    $"Player {player.Id} already has an active recruitment officer ({existingActiveOfficer.Id}). " +
                    "Their career must end (resign, retire, be fired, or die) before a new one can be started.");
            }

            var officer = new Domain.Entities.RecruitmentOfficer(
                Guid.NewGuid(),
                request.FullName,
                request.Department,
                player.Id);

            _db.AddRecruitmentOfficer(officer);

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Player {PlayerId} started a new game as recruitment officer {OfficerId}.",
                player.Id,
                officer.Id);

            return officer.Id;
        }
    }
}
