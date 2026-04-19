using Microsoft.EntityFrameworkCore;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Infrastructure.Persistence.Transactions;

namespace MilitaryDraftSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Citizen> Citizens => Set<Citizen>();

        // for supporting design-time tools like EF migrations
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public async Task<IAppTransaction> BeginTransactionAsync(CancellationToken ct)
        {
            // Start EF transaction
            var transaction = await Database.BeginTransactionAsync(ct);

            // Wrap EF transaction into abstraction
            return new EfTransaction(transaction);
        }

        public async Task<Citizen?> GetCitizenWithSummons(Guid id, CancellationToken ct)
        {
            return await Citizens
                .Include(x => x.Summonses)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
