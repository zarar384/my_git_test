using MilitaryDraftSystem.Application.Common.Configuration;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Population.Services.Interfaces;
using MilitaryDraftSystem.Domain.Common.RandomEvents;
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
        private static readonly IReadOnlyList<WeightedOutcome<MedicalCategory>> MedicalCategoryOutcomes =
        [
            new("fit", "Fit", SimulationProbabilities.MedicalCategory.FitWeight, MedicalCategory.Fit),
            new("limited-fit", "Limited Fit", SimulationProbabilities.MedicalCategory.LimitedFitWeight, MedicalCategory.LimitedFit),
            new("temporarily-unfit", "Temporarily Unfit", SimulationProbabilities.MedicalCategory.TemporarilyUnfitWeight, MedicalCategory.TemporarilyUnfit),
            new("permanently-unfit", "Permanently Unfit", SimulationProbabilities.MedicalCategory.PermanentlyUnfitWeight, MedicalCategory.PermanentlyUnfit)
        ];

        private static readonly IReadOnlyList<WeightedOutcome<AgeBracket>> AgeBracketOutcomes =
        [
            new("minor", "Minor", SimulationProbabilities.Age.MinorWeight, new AgeBracket(0, 18)),
            new("young-adult", "Young Adult", SimulationProbabilities.Age.YoungAdultWeight, new AgeBracket(18, 28)),
            new("middle-aged", "Middle Aged", SimulationProbabilities.Age.MiddleAgedWeight, new AgeBracket(28, 45)),
            new("elderly", "Elderly", SimulationProbabilities.Age.ElderlyWeight, new AgeBracket(45, 90))
        ];

        private readonly IRandomProvider _random;

        public PopulationGenerationService(IRandomProvider random)
        {
            _random = random;
        }

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

        public IReadOnlyCollection<Citizen> Generate(God god)
        {
            // Determine how many citizens will be created in this generation.
            var citizensToGenerate = _random.Next(
                god.MinCitizensPerGeneration,
                god.MaxCitizensPerGeneration + 1);

            var citizens = new List<Citizen>(citizensToGenerate);

            for (var i = 0; i < citizensToGenerate; i++)
            {
                citizens.Add(CreateCitizen(god));
            }

            return citizens;
        }

        private Citizen CreateCitizen(God god)
        {
            // Randomly determine the citizen's gender.
            var isMale = _random.Chance(50);

            var firstName = isMale
                ? _random.Pick(MaleFirstNames)
                : _random.Pick(FemaleFirstNames);

            var lastName = _random.Pick(LastNames);

            // Generate age using a weighted distribution, bounded by God's configured age range.
            var age = GenerateAge(god.MinAge, god.MaxAge);

            // Calculate birth date based on age and a random offset of up to 365 days.
            var birthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-age).AddDays(-_random.Next(0, 365)));

            var medicalCategory = GenerateMedicalCategory();

            var isStudent =
                age is >= 18 and <= 25 &&
                _random.Chance(god.StudentChance);

            var hasCriminalRecord =
                _random.Chance(god.CriminalRecordChance);

            var status = GenerateStatus(age, isStudent, hasCriminalRecord);

            var gender = isMale ? Gender.Male : Gender.Female;

            var citizen = new Citizen(
                Guid.NewGuid(),
                firstName,
                lastName,
                age,
                birthDate,
                medicalCategory,
                status,
                hasCriminalRecord,
                isStudent,
                gender);

            citizen.AssignNaturalLifespan(GenerateNaturalLifespan(gender));

            return citizen;
        }

        /// <summary>
        /// Samples a natural lifespan, in years, from a gender-specific normal
        /// distribution, clamped to a sane range. Life expectancy differs
        /// between men and women, so the simulation never uses a single fixed
        /// age of death.
        /// </summary>
        private int GenerateNaturalLifespan(Gender gender)
        {
            var (mean, stdDev) = gender == Gender.Female
                ? (SimulationProbabilities.Death.Lifespan.FemaleMeanYears, SimulationProbabilities.Death.Lifespan.FemaleStdDevYears)
                : (SimulationProbabilities.Death.Lifespan.MaleMeanYears, SimulationProbabilities.Death.Lifespan.MaleStdDevYears);

            var sampled = _random.NextGaussian(mean, stdDev);

            var clamped = Math.Clamp(
                sampled,
                SimulationProbabilities.Death.Lifespan.MinYears,
                SimulationProbabilities.Death.Lifespan.MaxYears);

            return (int)Math.Round(clamped);
        }

        /// <summary>
        /// Generates age using a non-uniform distribution defined by the weighted age bracket catalog,
        /// then clamps the result to God's configured minimum and maximum age. Younger citizens appear
        /// more frequently than older ones.
        /// </summary>
        private int GenerateAge(int minAge, int maxAge)
        {
            if (minAge > maxAge)
                throw new ArgumentException("God's minimum age cannot be greater than the maximum age.");

            var totalWeight = WeightedOutcomeSelector.TotalWeight(AgeBracketOutcomes);
            var roll = _random.NextDouble() * totalWeight;

            var bracket = WeightedOutcomeSelector.Select(AgeBracketOutcomes, roll);

            // Intersect the chosen bracket with God's configured age range.
            var minInclusive = Math.Max(bracket.MinInclusive, minAge);
            var maxExclusive = Math.Min(bracket.MaxExclusive, maxAge + 1);

            // If God's range excludes this bracket entirely, fall back to God's own range.
            if (minInclusive >= maxExclusive)
            {
                minInclusive = minAge;
                maxExclusive = maxAge + 1;
            }

            return _random.Next(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Generates the medical category using the weighted outcome catalog.
        /// </summary>
        private MedicalCategory GenerateMedicalCategory()
        {
            var totalWeight = WeightedOutcomeSelector.TotalWeight(MedicalCategoryOutcomes);
            var roll = _random.NextDouble() * totalWeight;

            return WeightedOutcomeSelector.Select(MedicalCategoryOutcomes, roll);
        }

        /// <summary>
        /// Determines the initial citizen status. Draft-age citizens may instead
        /// start with a temporary deferment or a permanent exemption, decided by
        /// the configured probabilities rather than a hardcoded rule.
        /// </summary>
        private CitizenStatus GenerateStatus(int age, bool isStudent, bool hasCriminalRecord)
        {
            if (age < 18)
                return CitizenStatus.Registered;

            if (age > 27)
                return CitizenStatus.Retired;

            // Students and citizens with a criminal record already fall outside the
            // normal draft pool, so life-event rolls only apply to otherwise-eligible citizens.
            if (!isStudent && !hasCriminalRecord)
            {
                if (_random.Chance(SimulationProbabilities.LifeEvents.ExemptionChancePercent))
                    return CitizenStatus.Exempted;

                if (_random.Chance(SimulationProbabilities.LifeEvents.DefermentChancePercent))
                    return CitizenStatus.Deferred;
            }

            return CitizenStatus.WaitingForDraft;
        }

        /// <summary>
        /// Represents an inclusive-exclusive age range used when generating a citizen's age.
        /// </summary>
        private readonly record struct AgeBracket(int MinInclusive, int MaxExclusive);
    }
}
