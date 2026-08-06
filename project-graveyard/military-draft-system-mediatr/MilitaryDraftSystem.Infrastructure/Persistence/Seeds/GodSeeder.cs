using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    /// <summary>
    /// Seeds God, the single autonomous entity responsible for creating new citizens.
    /// </summary>
    public static class GodSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Prevent duplicate seed data. There can only be one God.
            if (context.Gods.Any())
            {
                return;
            }

            var god = new God(
                id: Guid.NewGuid(),
                enabled: true,

                // Generate a new population every 30 seconds.
                generationInterval: TimeSpan.FromSeconds(30),

                // Create between 2 and 10 citizens per generation.
                minCitizensPerGeneration: 2,
                maxCitizensPerGeneration: 10,

                // Generate citizens between newborns and elderly people.
                minAge: 0,
                maxAge: 90,

                // Roughly one third of young citizens are students.
                studentChance: 35,

                // Criminal records should remain relatively rare.
                criminalRecordChance: 7);

            await context.Gods.AddAsync(god);

            await context.SaveChangesAsync();
        }
    }
}
