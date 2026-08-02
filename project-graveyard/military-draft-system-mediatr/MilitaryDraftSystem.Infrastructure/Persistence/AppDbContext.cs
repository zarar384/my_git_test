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

        public DbSet<PopulationGenerator> PopulationGenerators => Set<PopulationGenerator>();

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

        public async Task<AutomaticRecruitmentAgent?> GetAutomaticRecruitmentAgent(
            CancellationToken ct)
        {
            // Load the automatic recruitment configuration.
            return await AutomaticRecruitmentAgents
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PopulationGenerator?> GetPopulationGenerator(
            CancellationToken ct)
        {
            // Load the population generation configuration.
            return await PopulationGenerators
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(ct);
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
    }
}