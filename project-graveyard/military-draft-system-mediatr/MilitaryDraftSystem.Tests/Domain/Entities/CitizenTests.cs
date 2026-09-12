using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.Events;

namespace MilitaryDraftSystem.Tests.Domain.Entities
{
    public class CitizenTests
    {
        #region Constructor

        [Fact]
        public void Citizen_GenerateWithCorrectProperties_ShouldCreateNewbornCitizen()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Act
            var citizen = new Citizen(firstName, lastName, MedicalCategory.Fit);

            // Assert
            Assert.NotEqual(Guid.Empty, citizen.Id);
            Assert.Equal(firstName, citizen.FirstName);
            Assert.Equal(lastName, citizen.LastName);
            Assert.Equal(0, citizen.Age);
            Assert.Equal(today, citizen.BirthDate);
            Assert.Equal(CitizenStatus.Newborn, citizen.Status);
            Assert.False(citizen.HasCriminalRecord);
            Assert.False(citizen.IsStudent);
            Assert.True(citizen.IsAlive);
        }

        #endregion

        #region IsEligibleForDraft

        [Fact]
        public void IsEligibleForDraft_ShouldReturnTrueForEligibleCitizen()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Jane",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForCitizenUnder18()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 17,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnTrueForCitizenAge18()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 18,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnTrueForCitizenAge27()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 27,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForCitizenOver27()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 28,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForCitizenWithLimitedMedicalCategory()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.LimitedFit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForCitizenWithPermanentMedicalExemption()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.PermanentlyUnfit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForCitizenWithCriminalRecord()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: true,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForStudent()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: true);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForCitizenWithIncorrectStatus()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.Exempted,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEligibleForDraft_ShouldReturnFalseForDeceasedCitizen()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "John",
                lastName: "Smith",
                age: 20,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            citizen.Die(
                DeathReason.HeartAttack,
                DateTimeOffset.Now);

            // Act
            var result = citizen.IsEligibleForDraft(DateOnly.FromDateTime(DateTime.Now));

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Draft

        [Fact]
        public void Draft_ShouldChangeStatusToDrafted()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Alice",
                lastName: "Williams",
                age: 22,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            var agentId = Guid.NewGuid();

            // Act
            var summons = citizen.Draft(
                DraftSource.AutomaticAgent,
                officerId: null,
                automaticAgentId: agentId,
                draftedAt: DateTimeOffset.Now);

            // Assert
            Assert.Equal(CitizenStatus.Drafted, citizen.Status);
            Assert.NotNull(summons);
        }

        [Fact]
        public void Draft_ShouldCreateSummonsForCitizen()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Alice",
                lastName: "Williams",
                age: 22,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            var agentId = Guid.NewGuid();
            var draftedAt = DateTimeOffset.Now;

            // Act
            var summons = citizen.Draft(
                DraftSource.AutomaticAgent,
                officerId: null,
                automaticAgentId: agentId,
                draftedAt);

            // Assert
            Assert.Equal(citizen.Id, summons.CitizenId);
            Assert.Equal(DraftSource.AutomaticAgent, summons.Source);
            Assert.Null(summons.RecruitmentOfficerId);
            Assert.Equal(agentId, summons.AutomaticRecruitmentAgentId);
            Assert.Equal(draftedAt, summons.IssuedAt);
            Assert.Equal(SummonsStatus.Created, summons.Status);
        }

        [Fact]
        public void Draft_ShouldRaiseCitizenDraftedDomainEvent()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Alice",
                lastName: "Williams",
                age: 22,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.Draft(
                DraftSource.AutomaticAgent,
                officerId: null,
                automaticAgentId: Guid.NewGuid(),
                draftedAt: DateTimeOffset.Now);

            // Assert
            var domainEvent = Assert.Single(citizen.DomainEvents.OfType<CitizenDraftedDomainEvent>());

            Assert.Equal(citizen.Id, domainEvent.CitizenId);
        }

        [Fact]
        public void Draft_ShouldThrowExceptionForIneligibleCitizen()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Charlie",
                lastName: "Brown",
                age: 28,
                medicalCategory: MedicalCategory.LimitedFit,
                status: CitizenStatus.Exempted,
                hasCriminalRecord: true,
                isStudent: true);

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                citizen.Draft(
                    DraftSource.RecruitmentOfficer,
                    officerId: Guid.NewGuid(),
                    automaticAgentId: null,
                    draftedAt: DateTimeOffset.Now));

            // Assert
            Assert.Contains("not eligible", exception.Message);
            Assert.Equal(CitizenStatus.Exempted, citizen.Status);
        }

        #endregion

        #region HaveBirthday

        [Fact]
        public void HaveBirthday_ShouldIncreaseAgeByOne()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "David",
                lastName: "Miller",
                age: 25,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.HaveBirthday();

            // Assert
            Assert.Equal(26, citizen.Age);
        }

        [Fact]
        public void HaveBirthday_ShouldNotChangeAgeForDeceasedCitizen()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Eve",
                lastName: "Davis",
                age: 30,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.Deceased,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.HaveBirthday();

            // Assert
            Assert.Equal(30, citizen.Age);
        }

        [Fact]
        public void HaveBirthday_ShouldSetWaitingForDraftStatusWhenCitizenTurns18()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "David",
                lastName: "Miller",
                age: 17,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.Newborn,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.HaveBirthday();

            // Assert
            Assert.Equal(18, citizen.Age);
            Assert.Equal(CitizenStatus.WaitingForDraft, citizen.Status);
        }

        [Fact]
        public void HaveBirthday_ShouldRaiseCitizenBecameAdultDomainEventWhenCitizenTurns18()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "David",
                lastName: "Miller",
                age: 17,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.Newborn,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.HaveBirthday();

            // Assert
            var domainEvent = Assert.Single(citizen.DomainEvents.OfType<CitizenBecameAdultDomainEvent>());

            Assert.Equal(citizen.Id, domainEvent.CitizenId);
        }

        [Fact]
        public void HaveBirthday_ShouldRetireCitizenWhenCitizenBecomesOlderThan27()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "David",
                lastName: "Miller",
                age: 27,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.HaveBirthday();

            // Assert
            Assert.Equal(28, citizen.Age);
            Assert.Equal(CitizenStatus.Retired, citizen.Status);
        }

        [Fact]
        public void HaveBirthday_ShouldNotRetireCitizenWithAnotherStatus()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "David",
                lastName: "Miller",
                age: 27,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.Drafted,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.HaveBirthday();

            // Assert
            Assert.Equal(28, citizen.Age);
            Assert.Equal(CitizenStatus.Drafted, citizen.Status);
        }

        #endregion

        #region Die

        [Fact]
        public void Die_ShouldSetCitizenAsDeceased()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Frank",
                lastName: "Wilson",
                age: 40,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            var deathDate = DateTimeOffset.Now;

            // Act
            citizen.Die(DeathReason.Cancer, deathDate);

            // Assert
            Assert.False(citizen.IsAlive);
            Assert.Equal(CitizenStatus.Deceased, citizen.Status);
            Assert.NotNull(citizen.Death);
            Assert.Equal(DeathReason.Cancer, citizen.Death.Reason);
            Assert.Equal(deathDate, citizen.Death.OccurredAt);
        }

        [Fact]
        public void Die_ShouldRaiseCitizenDiedDomainEvent()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Grace",
                lastName: "Taylor",
                age: 50,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.WaitingForDraft,
                hasCriminalRecord: false,
                isStudent: false);

            var deathDate = DateTimeOffset.Now;
            var reason = DeathReason.HeartAttack;

            // Act
            citizen.Die(reason, deathDate);

            // Assert
            var domainEvent = Assert.Single(citizen.DomainEvents.OfType<CitizenDiedDomainEvent>());

            Assert.Equal(citizen.Id, domainEvent.CitizenId);
            Assert.Equal(reason, domainEvent.Death.Reason);
            Assert.Equal(deathDate, domainEvent.Death.OccurredAt);
        }

        [Fact]
        public void Die_ShouldDoNothingForAlreadyDeceasedCitizen()
        {
            // Arrange
            var citizen = new Citizen(
                firstName: "Grace",
                lastName: "Taylor",
                age: 50,
                medicalCategory: MedicalCategory.Fit,
                status: CitizenStatus.Deceased,
                hasCriminalRecord: false,
                isStudent: false);

            // Act
            citizen.Die(
                DeathReason.HeartAttack,
                DateTimeOffset.Now);

            // Assert
            Assert.Null(citizen.Death);
            Assert.Empty(citizen.DomainEvents);
        }

        #endregion
    }
}