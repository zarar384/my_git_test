using MilitaryDraftSystem.Domain.Common;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a recruitment officer responsible for manual drafting.
    /// </summary>
    public sealed class RecruitmentOfficer : Entity<Guid>
    {
        public string FullName { get; private set; } = null!;

        public string Department { get; private set; } = null!;
    }
}
