using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    /// <summary>
    /// Seeds a starter set of human recruitment officers so manual drafting can be exercised.
    /// </summary>
    public static class RecruitmentOfficerSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.RecruitmentOfficers.Any())
            {
                return; // Data already seeded
            }

            var officers = new List<RecruitmentOfficer>
            {
                new(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office", PlayerSeeder.DefaultPlayerId),
                new(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office")
            };

            await context.RecruitmentOfficers.AddRangeAsync(officers);
            await context.SaveChangesAsync();
        }
    }
}
