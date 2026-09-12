using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Tests.Domain.Entities
{
    public class RecruitmentOfficerLifecycleTests
    {
        [Fact]
        public void Constructor_ShouldStartActive()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            Assert.Equal(OfficerStatus.Active, officer.Status);
            Assert.True(officer.IsActive);
            Assert.False(officer.HasEndedCareer);
        }

        [Fact]
        public void GoOnLeave_ShouldSetStatusToOnLeave()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            officer.GoOnLeave(DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.OnLeave, officer.Status);
            Assert.False(officer.IsActive);
            Assert.False(officer.HasEndedCareer);
        }

        [Fact]
        public void ReturnFromLeave_ShouldRestoreActiveStatus()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            officer.GoOnLeave(DateTimeOffset.UtcNow);

            officer.ReturnFromLeave(DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.Active, officer.Status);
            Assert.True(officer.IsActive);
        }

        [Fact]
        public void ReturnFromLeave_ShouldThrow_WhenNotOnLeave()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            Assert.Throws<InvalidOperationException>(() => officer.ReturnFromLeave(DateTimeOffset.UtcNow));
        }

        [Fact]
        public void Resign_ShouldEndCareerWithResignationReason()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            officer.Resign(DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.Resigned, officer.Status);
            Assert.Equal(WorkerEndReason.Resignation, officer.EndReason);
            Assert.True(officer.HasEndedCareer);
        }

        [Fact]
        public void RetireVoluntarily_ShouldEndCareerWithRetirementReason()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            officer.RetireVoluntarily(DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.Retired, officer.Status);
            Assert.Equal(WorkerEndReason.Retirement, officer.EndReason);
            Assert.True(officer.IsRetired);
            Assert.True(officer.HasEndedCareer);
        }

        [Fact]
        public void Fire_ShouldEndCareerWithDismissalReason()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            officer.Fire(DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.Fired, officer.Status);
            Assert.Equal(WorkerEndReason.Dismissal, officer.EndReason);
            Assert.True(officer.HasEndedCareer);
        }

        [Fact]
        public void Die_ShouldSetDeceasedStatusAndDeathRecord()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            var occurredAt = DateTimeOffset.UtcNow;

            officer.Die(DeathReason.HeartAttack, occurredAt);

            Assert.Equal(OfficerStatus.Deceased, officer.Status);
            Assert.NotNull(officer.Death);
            Assert.Equal(DeathReason.HeartAttack, officer.Death!.Reason);
            Assert.True(officer.HasEndedCareer);
        }

        [Fact]
        public void DieOnDuty_ShouldSetDiedOnDutyReason()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");

            officer.DieOnDuty(DeathReason.KilledInCombat, DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.Deceased, officer.Status);
            Assert.Equal(WorkerEndReason.DiedOnDuty, officer.EndReason);
        }

        [Fact]
        public void EndCareer_ShouldBeIdempotent_WhenCareerAlreadyEnded()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            officer.Resign(DateTimeOffset.UtcNow);

            officer.Fire(DateTimeOffset.UtcNow);

            Assert.Equal(OfficerStatus.Resigned, officer.Status);
        }

        [Fact]
        public void GoOnLeave_ShouldThrow_WhenCareerAlreadyEnded()
        {
            var officer = new RecruitmentOfficer(Guid.NewGuid(), "Alan Turing", "Central Recruitment Office");
            officer.Resign(DateTimeOffset.UtcNow);

            Assert.Throws<InvalidOperationException>(() => officer.GoOnLeave(DateTimeOffset.UtcNow));
        }
    }
}
