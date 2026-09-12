using Microsoft.EntityFrameworkCore;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Infrastructure.Persistence.Transactions;

namespace MilitaryDraftSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Represents the application's database context.
    /// </summary>
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Citizen> Citizens => Set<Citizen>();

        public DbSet<Summons> Summonses => Set<Summons>();

        public DbSet<RecruitmentOfficer> RecruitmentOfficers => Set<RecruitmentOfficer>();

        public DbSet<AutomaticRecruitmentAgent> AutomaticRecruitmentAgents => Set<AutomaticRecruitmentAgent>();

        public DbSet<God> Gods => Set<God>();

        public DbSet<Player> Players => Set<Player>();

        public DbSet<CemeteryRecord> CemeteryRecords => Set<CemeteryRecord>();

        public DbSet<DeathStatistic> DeathStatistics => Set<DeathStatistic>();

        public DbSet<WorkerLifecycleStatistic> WorkerLifecycleStatistics => Set<WorkerLifecycleStatistic>();

        /// <summary>
        /// Required by EF Core design-time tools such as migrations.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct)
        {
            // Start a new database transaction.
            var transaction = await Database.BeginTransactionAsync(ct);

            // Wrap the EF Core transaction with the application abstraction.
            return new EfTransaction(transaction);
        }

        public async Task<Citizen?> GetCitizen(
            Guid id,
            CancellationToken ct)
        {
            // Load a citizen by its identifier.
            return await Citizens
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<List<Citizen>> GetCitizensEligibleForAutomaticDraft(
            CancellationToken ct)
        {
            // Load citizens that may participate in the automatic draft.
            return await Citizens
                .Where(x => x.Status == CitizenStatus.WaitingForDraft)
                .ToListAsync(ct);
        }

        public async Task<List<AutomaticRecruitmentAgent>> GetEnabledAutomaticRecruitmentAgents(
            CancellationToken ct)
        {
            // Load every automatic recruitment agent currently allowed to act.
            // Multiple agents may operate independently at the same time.
            return await AutomaticRecruitmentAgents
                .Where(x => x.Enabled)
                .ToListAsync(ct);
        }

        public async Task<God?> GetGod(
            CancellationToken ct)
        {
            // Load the population generation configuration. There can only be one God.
            return await Gods
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<Citizen>> GetLivingCitizens(
            CancellationToken ct)
        {
            // Load citizens that are still alive and part of the simulation.
            return await Citizens
                .Where(x => x.Status != CitizenStatus.Deceased)
                .ToListAsync(ct);
        }

        public async Task<List<Citizen>> GetDeceasedCitizensOlderThan(
            DateTimeOffset cutoff,
            CancellationToken ct)
        {
            // Load deceased citizens whose death happened at or before the cutoff,
            // making them eligible for physical removal from the population table.
            return await Citizens
                .Where(x => x.Status == CitizenStatus.Deceased && x.Death != null && x.Death.OccurredAt <= cutoff)
                .ToListAsync(ct);
        }

        public async Task<RecruitmentOfficer?> GetRecruitmentOfficer(
            Guid id,
            CancellationToken ct)
        {
            // Load a recruitment officer by its identifier.
            return await RecruitmentOfficers
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<List<RecruitmentOfficer>> GetRecruitmentOfficers(
            CancellationToken ct)
        {
            // Load every recruitment officer, active or not, so active and
            // inactive players can be distinguished by callers.
            return await RecruitmentOfficers
                .ToListAsync(ct);
        }

        public async Task<List<RecruitmentOfficer>> GetActiveRecruitmentOfficers(
            CancellationToken ct)
        {
            // Load only officers who may currently act, to drive the life simulation.
            return await RecruitmentOfficers
                .Where(x => x.Status == OfficerStatus.Active)
                .ToListAsync(ct);
        }

        public async Task<RecruitmentOfficer?> GetActiveRecruitmentOfficerByPlayer(
            Guid playerId,
            CancellationToken ct)
        {
            // A player may have only one officer whose career hasn't ended
            // (active or on leave) at a time.
            return await RecruitmentOfficers
                .Where(x => x.PlayerId == playerId)
                .Where(x =>
                    x.Status == OfficerStatus.Active ||
                    x.Status == OfficerStatus.OnLeave)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<Summons?> GetSummonsByCitizen(
            Guid citizenId,
            CancellationToken ct)
        {
            // Load the summons issued to a citizen, if any.
            return await Summonses
                .FirstOrDefaultAsync(x => x.CitizenId == citizenId, ct);
        }

        public async Task<Player?> GetPlayer(
            Guid id,
            CancellationToken ct)
        {
            // Load a player by its identifier.
            return await Players
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<CemeteryRecord?> GetCemeteryRecord(
            SubjectType subjectType,
            Guid subjectId,
            CancellationToken ct)
        {
            // Load the cemetery record for a given subject, regardless of whether
            // the original population row still exists.
            return await CemeteryRecords
                .FirstOrDefaultAsync(x => x.SubjectType == subjectType && x.SubjectId == subjectId, ct);
        }

        public async Task<List<CemeteryRecord>> GetCemeteryRecords(
            CancellationToken ct)
        {
            // Load every cemetery record, citizens and officers alike.
            return await CemeteryRecords
                .OrderByDescending(x => x.DiedAt)
                .ToListAsync(ct);
        }

        public async Task<DeathStatistic?> GetDeathStatistic(
            SubjectType subjectType,
            DeathReason reason,
            CancellationToken ct)
        {
            // Load the historical death counter for a given subject type and reason.
            return await DeathStatistics
                .FirstOrDefaultAsync(x => x.SubjectType == subjectType && x.Reason == reason, ct);
        }

        public async Task<List<DeathStatistic>> GetDeathStatistics(
            CancellationToken ct)
        {
            // Load every historical death counter.
            return await DeathStatistics
                .ToListAsync(ct);
        }

        public async Task<WorkerLifecycleStatistic?> GetWorkerLifecycleStatistic(
            WorkerEndReason reason,
            CancellationToken ct)
        {
            // Load the historical worker life-cycle counter for a given reason.
            return await WorkerLifecycleStatistics
                .FirstOrDefaultAsync(x => x.Reason == reason, ct);
        }

        public async Task<List<WorkerLifecycleStatistic>> GetWorkerLifecycleStatistics(
            CancellationToken ct)
        {
            // Load every historical worker life-cycle counter.
            return await WorkerLifecycleStatistics
                .ToListAsync(ct);
        }

        public void AddCitizen(Citizen citizen)
        {
            // Register a newly generated citizen.
            Citizens.Add(citizen);
        }

        public void AddSummons(Summons summons)
        {
            // Register a newly created summons.
            Summonses.Add(summons);
        }

        public void AddPlayer(Player player)
        {
            // Register a newly created player.
            Players.Add(player);
        }

        public void AddRecruitmentOfficer(RecruitmentOfficer officer)
        {
            // Register a newly created recruitment officer.
            RecruitmentOfficers.Add(officer);
        }

        public void AddCemeteryRecord(CemeteryRecord record)
        {
            // Register a new permanent cemetery record.
            CemeteryRecords.Add(record);
        }

        public void AddDeathStatistic(DeathStatistic statistic)
        {
            // Register a new death statistic counter (subject type + reason combination).
            DeathStatistics.Add(statistic);
        }

        public void AddWorkerLifecycleStatistic(WorkerLifecycleStatistic statistic)
        {
            // Register a new worker life-cycle statistic counter.
            WorkerLifecycleStatistics.Add(statistic);
        }

        public void RemoveCitizen(Citizen citizen)
        {
            // Physically remove the citizen from the population table. Their
            // historical footprint must already live in CemeteryRecord/DeathStatistic.
            Citizens.Remove(citizen);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the Death value object as an owned type on Citizen and RecruitmentOfficer.
            modelBuilder.Entity<Citizen>()
                .OwnsOne(x => x.Death);

            modelBuilder.Entity<RecruitmentOfficer>()
                .OwnsOne(x => x.Death);
        }
    }
}