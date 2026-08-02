using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.Events;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a citizen who may be eligible for military service.
    /// </summary>
    public sealed class Citizen : Entity<Guid>
    {
        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public int Age { get; set; }

        public DateOnly BirthDate { get; private set; }

        public MedicalCategory MedicalCategory { get; private set; }

        public CitizenStatus Status { get; private set; }

        public bool HasCriminalRecord { get; private set; }

        public bool IsStudent { get; private set; }

        public Citizen(
            Guid id,
            string firstName,
            string lastName,
            int age,
            DateOnly birthDate,
            MedicalCategory medicalCategory,
            CitizenStatus status,
            bool hasCriminalRecord,
            bool isStudent)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            BirthDate = birthDate;
            MedicalCategory = medicalCategory;
            Status = status;
            HasCriminalRecord = hasCriminalRecord;
            IsStudent = isStudent;
        }

        public Citizen()
        {
            // Required by EF Core.
        }

        public bool IsEligibleForDraft(DateOnly today)
        {
            // Check whether the citizen satisfies all draft eligibility requirements.
            return Age >= 18
                && Age <= 27
                && MedicalCategory == MedicalCategory.Fit
                && !HasCriminalRecord
                && !IsStudent
                && Status == CitizenStatus.WaitingForDraft;
        }

        public Summons Draft(
            DraftSource source,
            Guid? officerId,
            Guid? automaticAgentId,
            DateTimeOffset draftedAt)
        {
            // Ensure the citizen can legally be drafted.
            if (!IsEligibleForDraft(DateOnly.FromDateTime(draftedAt.UtcDateTime)))
                throw new InvalidOperationException("Citizen is not eligible for military draft.");

            // Update the citizen's state.
            Status = CitizenStatus.Drafted;

            // Create a new summons for the drafted citizen.
            var summons = Summons.Create(
                Id,
                source,
                officerId,
                automaticAgentId,
                draftedAt);

            // Notify the system that the citizen has been drafted.
            RaiseDomainEvent(new CitizenDraftedDomainEvent(Id));

            return summons;
        }
    }
}
