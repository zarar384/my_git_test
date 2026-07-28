using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    public static class CitizenSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));

            var citizens = new List<Citizen>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    Age = 21,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Mary Smith",
                    Age = 18,
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Michael Johnson",
                    Age = 25,
                }
            };

            await context.Citizens.AddRangeAsync(citizens);
            await context.SaveChangesAsync();
        }
    }
}
