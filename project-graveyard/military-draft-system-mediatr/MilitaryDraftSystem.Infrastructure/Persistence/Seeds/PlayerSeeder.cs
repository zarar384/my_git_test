using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    /// <summary>
    /// Seeds the current single player of the game. The model is designed so
    /// multiple players can exist in the future, but today only one is seeded.
    /// </summary>
    public static class PlayerSeeder
    {
        public static readonly Guid DefaultPlayerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static async Task SeedAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Players.Any())
            {
                return; // Data already seeded
            }

            var player = new Player(DefaultPlayerId, "Player One", DateTimeOffset.UtcNow);

            await context.Players.AddAsync(player);
            await context.SaveChangesAsync();
        }
    }
}
