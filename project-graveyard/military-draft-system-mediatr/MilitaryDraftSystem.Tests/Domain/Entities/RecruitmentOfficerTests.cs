using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Tests.Domain.Entities
{
    public class RecruitmentOfficerTests
    {
        [Fact]
        public void RegisterDraftedCitizen_ShouldIncrementCounter()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            // Act
            officer.RegisterDraftedCitizen();
            officer.RegisterDraftedCitizen();

            // Assert
            Assert.Equal(2, officer.DraftedCitizensCount);
        }

        [Fact]
        public void Constructor_ShouldStartWithZeroDraftedCitizens()
        {
            // Arrange & Act
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");

            // Assert
            Assert.Equal(0, officer.DraftedCitizensCount);
        }

        [Fact]
        public void Constructor_ShouldStartWithFullMoraleAndNoGuilt()
        {
            // Arrange & Act
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");

            // Assert
            Assert.Equal(100, officer.MoralePercent);
            Assert.Equal(0, officer.GuiltIncidentsCount);
        }

        [Fact]
        public void ApplyGuilt_ShouldReduceMoraleAndIncrementGuiltIncidents()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            // Act
            officer.ApplyGuilt(15);

            // Assert
            Assert.Equal(85, officer.MoralePercent);
            Assert.Equal(1, officer.GuiltIncidentsCount);
        }

        [Fact]
        public void ApplyGuilt_ShouldNotReduceMoraleBelowZero()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            // Act
            officer.ApplyGuilt(60);
            officer.ApplyGuilt(60);

            // Assert
            Assert.Equal(0, officer.MoralePercent);
            Assert.Equal(2, officer.GuiltIncidentsCount);
        }

        [Fact]
        public void ApplyGuilt_ShouldThrowForNegativeAmount()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => officer.ApplyGuilt(-1));
        }

        [Fact]
        public void ApplyGuilt_ShouldRetireOfficer_WhenMoraleReachesZero()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            // Act
            officer.ApplyGuilt(100);

            // Assert
            Assert.Equal(0, officer.MoralePercent);
            Assert.True(officer.IsRetired);
        }

        [Fact]
        public void ApplyGuilt_ShouldNotRetireOfficer_WhenMoraleIsAboveZero()
        {
            // Arrange
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            // Act
            officer.ApplyGuilt(15);

            // Assert
            Assert.False(officer.IsRetired);
        }

        [Fact]
        public void Constructor_ShouldStartNotRetired()
        {
            // Arrange & Act
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Grace Hopper", "Central Recruitment Office");

            // Assert
            Assert.False(officer.IsRetired);
        }
    }
}
