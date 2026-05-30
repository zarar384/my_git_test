using Microsoft.EntityFrameworkCore.Storage;
using MilitaryDraftSystem.Application.Common.Interfaces;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Transactions
{
    // Adapter class to wrap EF Core transactions and implement IAppTransaction
    // This allows the application layer to work with a consistent transaction interface, regardless of the underlying implementation
    public class EfTransaction : IAppTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        // Commit transaction
        public Task CommitAsync(CancellationToken ct)
            => _transaction.CommitAsync(ct);

        // Rollback transaction
        public Task RollbackAsync(CancellationToken ct)
            => _transaction.RollbackAsync(ct);

        // Dispose transaction
        public ValueTask DisposeAsync()
            => _transaction.DisposeAsync();
    }
}
