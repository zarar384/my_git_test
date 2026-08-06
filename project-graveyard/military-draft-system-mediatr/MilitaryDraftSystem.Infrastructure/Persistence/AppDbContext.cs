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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map the Death value object as an owned type on Citizen.
            modelBuilder.Entity<Citizen>()
                .OwnsOne(x => x.Death);
        }
    }
}