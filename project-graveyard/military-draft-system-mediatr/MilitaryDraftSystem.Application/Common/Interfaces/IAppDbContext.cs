using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Provides application-specific database operations.
    /// </summary>
    public interface IAppDbContext
    {
        // Queries

        Task<Citizen?> GetCitizen(Guid id, CancellationToken ct);

        Task<List<Citizen>> GetCitizensEligibleForAutomaticDraft(CancellationToken ct);

        Task<List<AutomaticRecruitmentAgent>> GetEnabledAutomaticRecruitmentAgents(CancellationToken ct);

        Task<God?> GetGod(CancellationToken ct);

        Task<List<Citizen>> GetLivingCitizens(CancellationToken ct);

        /// <summary>
        /// Loads citizens who died at or before the given cutoff, and are
        /// therefore eligible to be physically purged from the population table.
        /// </summary>
        Task<List<Citizen>> GetDeceasedCitizensOlderThan(DateTimeOffset cutoff, CancellationToken ct);

        Task<RecruitmentOfficer?> GetRecruitmentOfficer(Guid id, CancellationToken ct);

        /// <summary>
        /// Loads every recruitment officer, active or not, so the system can
        /// distinguish active players from those who have ended their career.
        /// </summary>
        Task<List<RecruitmentOfficer>> GetRecruitmentOfficers(CancellationToken ct);

        /// <summary>
        /// Loads every recruitment officer currently in active duty, used to
        /// drive the officer life simulation.
        /// </summary>
        Task<List<RecruitmentOfficer>> GetActiveRecruitmentOfficers(CancellationToken ct);

        /// <summary>
        /// Loads the recruitment officer currently controlled by the given
        /// player whose career has not yet ended (active or on leave), if
        /// any. A player may only have one such officer at a time, so this is
        /// used to enforce that rule when a new career is started.
        /// </summary>
        Task<RecruitmentOfficer?> GetActiveRecruitmentOfficerByPlayer(Guid playerId, CancellationToken ct);

        Task<Summons?> GetSummonsByCitizen(Guid citizenId, CancellationToken ct);

        Task<Player?> GetPlayer(Guid id, CancellationToken ct);

        Task<CemeteryRecord?> GetCemeteryRecord(SubjectType subjectType, Guid subjectId, CancellationToken ct);

        Task<List<CemeteryRecord>> GetCemeteryRecords(CancellationToken ct);

        Task<DeathStatistic?> GetDeathStatistic(SubjectType subjectType, DeathReason reason, CancellationToken ct);

        Task<List<DeathStatistic>> GetDeathStatistics(CancellationToken ct);

        Task<WorkerLifecycleStatistic?> GetWorkerLifecycleStatistic(WorkerEndReason reason, CancellationToken ct);

        Task<List<WorkerLifecycleStatistic>> GetWorkerLifecycleStatistics(CancellationToken ct);

        // Commands

        void AddCitizen(Citizen citizen);

        void AddSummons(Summons summons);

        void AddPlayer(Player player);

        void AddRecruitmentOfficer(RecruitmentOfficer officer);

        void AddCemeteryRecord(CemeteryRecord record);

        void AddDeathStatistic(DeathStatistic statistic);

        void AddWorkerLifecycleStatistic(WorkerLifecycleStatistic statistic);

        /// <summary>
        /// Physically removes a deceased citizen from the population table.
        /// Their historical footprint must have already been preserved via
        /// <see cref="CemeteryRecord"/> and <see cref="DeathStatistic"/>.
        /// </summary>
        void RemoveCitizen(Citizen citizen);

        // Persistence

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // Transactions

        Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct);
    }
}
