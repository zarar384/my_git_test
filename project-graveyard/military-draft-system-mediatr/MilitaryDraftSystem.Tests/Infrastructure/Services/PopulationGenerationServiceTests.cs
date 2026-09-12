using MilitaryDraftSystem.Application.Population.Services;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Tests.Common;

namespace MilitaryDraftSystem.Tests.Infrastructure.Services
{
    public class PopulationGenerationServiceTests
    {
        private static God CreateGod(
            int minCitizens = 1,
            int maxCitizens = 1,
            int studentChance = 0,
            int criminalRecordChance = 0)
        {
            return new God(
                Guid.NewGuid(),
                enabled: true,
                generationInterval: TimeSpan.FromMinutes(1),
                minCitizensPerGeneration: minCitizens,
                maxCitizensPerGeneration: maxCitizens,
                minAge: 0,
                maxAge: 90,
                studentChance: studentChance,
                criminalRecordChance: criminalRecordChance);
        }

        [Fact]
        public void Generate_ShouldCreateExactlyOneCitizen_WhenMinAndMaxAreEqual()
        {
            // Arrange
            var random = new FakeRandomProvider { DefaultInt = 0, DefaultDouble = 0 };
            var god = CreateGod(minCitizens: 1, maxCitizens: 1);
            var sut = new PopulationGenerationService(random);

            // Act
            var citizens = sut.Generate(god);

            // Assert
            Assert.Single(citizens);
        }

        [Fact]
        public void Generate_ShouldCreateRequestedNumberOfCitizens_WhenRangeIsFixedByRandomProvider()
        {
            // Arrange
            var random = new FakeRandomProvider { DefaultDouble = 0 };
            random.EnqueueInt(3); // citizensToGenerate
            var god = CreateGod(minCitizens: 1, maxCitizens: 5);
            var sut = new PopulationGenerationService(random);

            // Act
            var citizens = sut.Generate(god);

            // Assert
            Assert.Equal(3, citizens.Count);
        }

        [Fact]
        public void Generate_ShouldAssignFitMedicalCategory_WhenRandomRollIsLow()
        {
            // Arrange: roll of 0.0 falls in the lowest weight bucket (Fit).
            var random = new FakeRandomProvider { DefaultInt = 0, DefaultDouble = 0 };
            var god = CreateGod();
            var sut = new PopulationGenerationService(random);

            // Act
            var citizen = sut.Generate(god).Single();

            // Assert
            Assert.Equal(MedicalCategory.Fit, citizen.MedicalCategory);
        }

        [Fact]
        public void Generate_ShouldAssignPermanentlyUnfitMedicalCategory_WhenRandomRollIsHigh()
        {
            // Arrange: NextDouble is consumed in order (isMale, age bracket, medical category).
            // Queue neutral rolls for the first two calls, then a high roll for medical category
            // so it lands in the highest weight bucket (Permanently Unfit).
            var random = new FakeRandomProvider { DefaultInt = 0 };
            random.EnqueueDouble(0);
            random.EnqueueDouble(0);
            random.EnqueueDouble(0.999);
            var god = CreateGod();
            var sut = new PopulationGenerationService(random);

            // Act
            var citizen = sut.Generate(god).Single();

            // Assert
            Assert.Equal(MedicalCategory.PermanentlyUnfit, citizen.MedicalCategory);
        }

        [Fact]
        public void Generate_ShouldExemptDraftAgeCitizen_WhenExemptionRollSucceeds()
        {
            // Arrange: force a draft-age (26), non-student, non-criminal citizen, then
            // make the exemption roll succeed (ExemptionChancePercent = 4).
            // Call order: Generate() first rolls citizensToGenerate(int), then CreateCitizen rolls:
            // gender(double), firstName pick(int), lastName pick(int), age bracket roll(double),
            // age exact(int), birthday offset(int), medical category(double), hasCriminalRecord(double)
            // [isStudent chance skipped: age 26 is outside 18-25], exemption(double).
            var random = new FakeRandomProvider();
            random.EnqueueInt(1);      // citizensToGenerate
            random.EnqueueDouble(0);   // gender
            random.EnqueueInt(0);      // firstName pick
            random.EnqueueInt(0);      // lastName pick
            random.EnqueueDouble(0.5); // age bracket roll -> Young Adult (18-28)
            random.EnqueueInt(26);     // exact age within bracket
            random.EnqueueInt(5);      // birth date day offset
            random.EnqueueDouble(0);   // medical category roll -> Fit
            random.EnqueueDouble(0);   // hasCriminalRecord roll -> false (chance = 0)
            random.EnqueueDouble(0);   // exemption roll -> succeeds (0 < 4)

            var god = CreateGod(studentChance: 0, criminalRecordChance: 0);
            var sut = new PopulationGenerationService(random);

            // Act
            var citizen = sut.Generate(god).Single();

            // Assert
            Assert.Equal(CitizenStatus.Exempted, citizen.Status);
        }

        [Fact]
        public void Generate_ShouldDeferDraftAgeCitizen_WhenExemptionFailsButDefermentRollSucceeds()
        {
            // Arrange: same draft-age citizen, but exemption fails and deferment succeeds.
            var random = new FakeRandomProvider();
            random.EnqueueInt(1);      // citizensToGenerate
            random.EnqueueDouble(0);   // gender
            random.EnqueueInt(0);      // firstName pick
            random.EnqueueInt(0);      // lastName pick
            random.EnqueueDouble(0.5); // age bracket roll -> Young Adult (18-28)
            random.EnqueueInt(26);     // exact age within bracket
            random.EnqueueInt(5);      // birth date day offset
            random.EnqueueDouble(0);   // medical category roll -> Fit
            random.EnqueueDouble(0);   // hasCriminalRecord roll -> false
            random.EnqueueDouble(0.5); // exemption roll -> fails (50 >= 4)
            random.EnqueueDouble(0);   // deferment roll -> succeeds (0 < 8)

            var god = CreateGod(studentChance: 0, criminalRecordChance: 0);
            var sut = new PopulationGenerationService(random);

            // Act
            var citizen = sut.Generate(god).Single();

            // Assert
            Assert.Equal(CitizenStatus.Deferred, citizen.Status);
        }

        [Fact]
        public void Generate_ShouldWaitForDraft_WhenExemptionAndDefermentRollsBothFail()
        {
            // Arrange: same draft-age citizen, but both life-event rolls fail.
            var random = new FakeRandomProvider();
            random.EnqueueInt(1);      // citizensToGenerate
            random.EnqueueDouble(0);   // gender
            random.EnqueueInt(0);      // firstName pick
            random.EnqueueInt(0);      // lastName pick
            random.EnqueueDouble(0.5); // age bracket roll -> Young Adult (18-28)
            random.EnqueueInt(26);     // exact age within bracket
            random.EnqueueInt(5);      // birth date day offset
            random.EnqueueDouble(0);   // medical category roll -> Fit
            random.EnqueueDouble(0);   // hasCriminalRecord roll -> false
            random.EnqueueDouble(0.5); // exemption roll -> fails
            random.EnqueueDouble(0.5); // deferment roll -> fails

            var god = CreateGod(studentChance: 0, criminalRecordChance: 0);
            var sut = new PopulationGenerationService(random);

            // Act
            var citizen = sut.Generate(god).Single();

            // Assert
            Assert.Equal(CitizenStatus.WaitingForDraft, citizen.Status);
        }
    }
}
