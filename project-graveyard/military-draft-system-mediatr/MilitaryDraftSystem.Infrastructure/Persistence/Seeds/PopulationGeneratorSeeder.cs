using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    /// <summary>
    /// Seeds the default population generator configuration.
    /// </summary>
    public static class PopulationGeneratorSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Prevent duplicate seed data.
            if (context.PopulationGenerators.Any())
            {
                return;
            }

            var generator = new PopulationGenerator(
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

            await context.PopulationGenerators.AddAsync(generator);

            await context.SaveChangesAsync();
        }
    }
}
