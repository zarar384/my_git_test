using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Infrastructure.Persistence.Seeds
{
    public static class CitizenSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var citizens = new List<Citizen>
            {
                new(
                    Guid.NewGuid(),
                    "John",
                    "Doe",
                    21,
                    new DateOnly(2005, 1, 15),
                    MedicalCategory.Fit,
                    CitizenStatus.WaitingForDraft,
                    hasCriminalRecord: false,
                    isStudent: false),

                new(
                    Guid.NewGuid(),
                    "Mary",
                    "Smith",
                    18,
                    new DateOnly(2008, 3, 20),
                    MedicalCategory.Fit,
                    CitizenStatus.WaitingForDraft,
                    hasCriminalRecord: false,
                    isStudent: true),

                new(
                    Guid.NewGuid(),
                    "Michael",
                    "Johnson",
                    25,
                    new DateOnly(2001, 7, 8),
                    MedicalCategory.LimitedFit,
                    CitizenStatus.WaitingForDraft,
                    hasCriminalRecord: false,
                    isStudent: false)
            };

            await context.Citizens.AddRangeAsync(citizens);
            await context.SaveChangesAsync();
        }
    }
}
