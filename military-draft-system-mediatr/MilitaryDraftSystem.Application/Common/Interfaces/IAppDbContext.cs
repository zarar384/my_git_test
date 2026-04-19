using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        // Encapsulate query
        Task<Citizen?> GetCitizenWithSummons(Guid id, CancellationToken ct);

        // Save changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // Begin transaction
        Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct);
    }
}
