using MilitaryDraftSystem.Application.Population.Services.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Application.Population.Services
{
    /// <summary>
    /// Creates a random population using weighted probability rules.
    /// </summary>
    public sealed class PopulationGenerationService
        : IPopulationGenerationService
    {
        private static readonly Random Random = Random.Shared;

        // TODO: Replace with a json file or database for more realistic data.
        private static readonly string[] MaleFirstNames =
        [
            "John",
            "Michael",
            "David",
            "James",
            "Robert",
            "Daniel",
            "William",
            "Thomas",
            "Richard",
            "Andrew",
            "Alexander",
            "Peter",
            "Samuel",
            "Nicholas",
            "Edward"
        ];

        private static readonly string[] FemaleFirstNames =
        [
            "Emma",
            "Olivia",
            "Sophia",
            "Charlotte",
            "Emily",
            "Sarah",
            "Anna",
            "Elizabeth",
            "Grace",
            "Victoria",
            "Alice",
            "Julia",
            "Amelia",
            "Isabella",
            "Eva"
        ];

        private static readonly string[] LastNames =
        [
            "Smith",
            "Johnson",
            "Brown",
            "Williams",
            "Miller",
            "Davis",
            "Wilson",
            "Taylor",
            "Anderson",
            "Thomas",
            "White",
            "Moore",
            "Martin",
            "Jackson",
            "Walker"
        ];

        public IReadOnlyCollection<Citizen> Generate(PopulationGenerator generator)
        {
            // Determine how many citizens will be created in this generation.
            var citizensToGenerate = Random.Next(
                generator.MinCitizensPerGeneration,
                generator.MaxCitizensPerGeneration + 1);

            var citizens = new List<Citizen>(citizensToGenerate);

            for (var i = 0; i < citizensToGenerate; i++)
            {
                citizens.Add(CreateCitizen(generator));
            }

            return citizens;
        }

        private Citizen CreateCitizen(PopulationGenerator generator)
        {
            // Randomly determine the citizen's gender.
            var isMale = Chance(50);

            var firstName = isMale
                ? Pick(MaleFirstNames)
                : Pick(FemaleFirstNames);

            var lastName = Pick(LastNames);

            // Generate age using a weighted distribution.
            var age = GenerateAge(generator);

            // Calculate birth date based on age and a random offset of up to 365 days.
            var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-age).AddDays(-Random.Next(365)));

            var medicalCategory = GenerateMedicalCategory();

            var isStudent =
                age is >= 18 and <= 25 &&
                Chance(generator.StudentChance);

            var hasCriminalRecord =
                Chance(generator.CriminalRecordChance);

            var status = GenerateStatus(age);

            return new Citizen(
                Guid.NewGuid(),
                firstName,
                lastName,
                age,
                birthDate,
                medicalCategory,
                status,
                hasCriminalRecord,
                isStudent);
        }

        /// <summary>
        /// Generates age using a non-uniform distribution.
        /// Younger citizens appear more frequently than older ones.
        /// </summary>
        private static int GenerateAge(PopulationGenerator generator)
        {
            var roll = Random.Next(100);

            return roll switch
            {
                < 15 => Random.Next(0, 18),
                < 70 => Random.Next(18, 28),
                < 90 => Random.Next(28, 45),
                _ => Random.Next(45, 90)
            };
        }

        /// <summary>
        /// Generates the medical category using weighted probabilities.
        /// </summary>
        private static MedicalCategory GenerateMedicalCategory()
        {
            var roll = Random.Next(100);

            return roll switch
            {
                < 70 => MedicalCategory.Fit,
                < 85 => MedicalCategory.LimitedFit,
                < 95 => MedicalCategory.TemporarilyUnfit,
                _ => MedicalCategory.PermanentlyUnfit
            };
        }

        /// <summary>
        /// Determines the initial citizen status.
        /// </summary>
        private static CitizenStatus GenerateStatus(int age)
        {
            if (age < 18)
                return CitizenStatus.Registered;

            if (age <= 27)
                return CitizenStatus.WaitingForDraft;

            return CitizenStatus.Retired;
        }

        /// <summary>
        /// Returns true according to the specified probability.
        /// </summary>
        private static bool Chance(int percent)
        {
            return Random.Next(100) < percent;
        }

        /// <summary>
        /// Selects a random value from the specified array.
        /// </summary>
        private static T Pick<T>(T[] values)
        {
            return values[Random.Next(values.Length)];
        }
    }
}
