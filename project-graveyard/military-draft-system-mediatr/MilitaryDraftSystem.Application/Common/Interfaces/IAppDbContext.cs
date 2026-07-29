using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        // Queries
        Task<Citizen?> GetCitizenWithSummons(Guid id, CancellationToken ct);

        Task<List<Citizen>> GetCitizensEligibleForAutomaticDraft(CancellationToken ct);

        Task<AutomaticRecruitmentAgent?> GetAutomaticRecruitmentAgent(CancellationToken ct);

        // Commands
        void AddSummons(Summons summons);

        // Persistence
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // Transactions
        Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct);
    }
}
