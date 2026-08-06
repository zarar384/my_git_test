using MilitaryDraftSystem.Domain.Entities;

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

        // Commands

        void AddCitizen(Citizen citizen);

        void AddSummons(Summons summons);

        // Persistence

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // Transactions

        Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct);
    }
}
