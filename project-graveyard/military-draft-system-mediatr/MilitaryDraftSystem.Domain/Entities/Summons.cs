using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a military summons issued to a citizen.
    /// </summary>
    public sealed class Summons : Entity<Guid>
    {
        public Guid CitizenId { get; private set; }

        public DraftSource Source { get; private set; }

        public Guid? RecruitmentOfficerId { get; private set; }

        public Guid? AutomaticRecruitmentAgentId { get; private set; }

        public SummonsStatus Status { get; private set; }

        public DateTimeOffset IssuedAt { get; private set; }

        private Summons()
        {
            // Required by EF Core.
        }

        public static Summons Create(
            Guid citizenId,
            DraftSource source,
            Guid? recruitmentOfficerId,
            Guid? automaticRecruitmentAgentId,
            DateTimeOffset issuedAt)
        {
            // Create a new summons with its initial state.
            return new Summons
            {
                Id = Guid.NewGuid(),
                CitizenId = citizenId,
                Source = source,
                RecruitmentOfficerId = recruitmentOfficerId,
                AutomaticRecruitmentAgentId = automaticRecruitmentAgentId,
                IssuedAt = issuedAt,
                Status = SummonsStatus.Created
            };
        }
    }
}
