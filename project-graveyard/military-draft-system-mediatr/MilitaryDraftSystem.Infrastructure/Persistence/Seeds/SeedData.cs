namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    public static class SeedData
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            // Seed citizens
            await CitizenSeeder.SeedAsync(context);

            // Seed God
            await GodSeeder.SeedAsync(context);

            // Seed the current player
            await PlayerSeeder.SeedAsync(context);

            // Seed recruitment officers
            await RecruitmentOfficerSeeder.SeedAsync(context);
        }
    }
}
