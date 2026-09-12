using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// A permanent record of a death, independent of whether the original
    /// <see cref="Citizen"/> or <see cref="RecruitmentOfficer"/> row still
    /// exists. The cemetery is built entirely from these records, so deleting
    /// the original entity from the population table never erases history.
    /// </summary>
    public sealed class CemeteryRecord : Entity<Guid>
    {
        public SubjectType SubjectType { get; private set; }

        public Guid SubjectId { get; private set; }

        public string FullName { get; private set; } = null!;

        public DeathReason Reason { get; private set; }

        public DateTimeOffset DiedAt { get; private set; }

        /// <summary>
        /// Whether the original population row has since been physically removed.
        /// </summary>
        public bool OriginalRecordDeleted { get; private set; }

        private CemeteryRecord()
        {
            // Required by EF Core.
        }

        public static CemeteryRecord Create(
            SubjectType subjectType,
            Guid subjectId,
            string fullName,
            DeathReason reason,
            DateTimeOffset diedAt)
        {
            return new CemeteryRecord
            {
                Id = Guid.NewGuid(),
                SubjectType = subjectType,
                SubjectId = subjectId,
                FullName = fullName,
                Reason = reason,
                DiedAt = diedAt,
                OriginalRecordDeleted = false
            };
        }

        /// <summary>
        /// Marks that the original population row backing this record has been
        /// physically deleted. The cemetery record itself is never deleted.
        /// </summary>
        public void MarkOriginalRecordDeleted()
        {
            OriginalRecordDeleted = true;
        }
    }
}
