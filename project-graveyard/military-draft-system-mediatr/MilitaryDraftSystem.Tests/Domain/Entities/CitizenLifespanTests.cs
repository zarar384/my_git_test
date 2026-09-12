using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Domain.Entities
{
    public class CitizenLifespanTests
    {
        private static Citizen CreateCitizen(int age = 10)
        {
            return new Citizen(
                firstName: "John",
                lastName: "Doe",
                age: age,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);
        }

        [Fact]
        public void AssignNaturalLifespan_ShouldSetValue_WhenNotAlreadyAssigned()
        {
            var citizen = CreateCitizen();

            citizen.AssignNaturalLifespan(80);

            Assert.Equal(80, citizen.NaturalLifespanYears);
        }

        [Fact]
        public void AssignNaturalLifespan_ShouldNotOverwrite_WhenAlreadyAssigned()
        {
            var citizen = CreateCitizen();
            citizen.AssignNaturalLifespan(80);

            citizen.AssignNaturalLifespan(50);

            Assert.Equal(80, citizen.NaturalLifespanYears);
        }

        [Fact]
        public void AssignNaturalLifespan_ShouldThrow_ForNonPositiveValue()
        {
            var citizen = CreateCitizen();

            Assert.Throws<ArgumentOutOfRangeException>(() => citizen.AssignNaturalLifespan(0));
        }

        [Fact]
        public void HasReachedNaturalLifespan_ShouldReturnFalse_WhenLifespanNotAssigned()
        {
            var citizen = CreateCitizen(age: 90);

            Assert.False(citizen.HasReachedNaturalLifespan());
        }

        [Fact]
        public void HasReachedNaturalLifespan_ShouldReturnFalse_WhenBelowAssignedLifespan()
        {
            var citizen = CreateCitizen(age: 40);
            citizen.AssignNaturalLifespan(80);

            Assert.False(citizen.HasReachedNaturalLifespan());
        }

        [Fact]
        public void HasReachedNaturalLifespan_ShouldReturnTrue_WhenAgeMeetsOrExceedsLifespan()
        {
            var citizen = CreateCitizen(age: 80);
            citizen.AssignNaturalLifespan(80);

            Assert.True(citizen.HasReachedNaturalLifespan());
        }

        [Fact]
        public void Die_ShouldMarkCitizenDeceasedWithReason()
        {
            var citizen = CreateCitizen();
            var occurredAt = DateTimeOffset.UtcNow;

            citizen.Die(DeathReason.OldAge, occurredAt);

            Assert.False(citizen.IsAlive);
            Assert.Equal(CitizenStatus.Deceased, citizen.Status);
            Assert.NotNull(citizen.Death);
            Assert.Equal(DeathReason.OldAge, citizen.Death!.Reason);
        }

        [Fact]
        public void Die_ShouldBeIdempotent_WhenAlreadyDeceased()
        {
            var citizen = CreateCitizen();
            citizen.Die(DeathReason.OldAge, DateTimeOffset.UtcNow);
            var firstDeath = citizen.Death;

            citizen.Die(DeathReason.HeartAttack, DateTimeOffset.UtcNow);

            Assert.Equal(firstDeath!.Reason, citizen.Death!.Reason);
        }
    }
}
