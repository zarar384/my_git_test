using MilitaryDraftSystem.Domain.Common;
using MilitaryDraftSystem.Domain.Enums;
using MilitaryDraftSystem.Domain.Events;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a citizen who may be eligible for military service.
    /// </summary>
    public sealed class Citizen : Entity<Guid>
    {
        /// <summary>
        /// Age at which a citizen is considered an adult.
        /// </summary>
        public const int AdultAge = 18;

        public string FirstName { get; private set; } = null!;

        public string LastName { get; private set; } = null!;

        public int Age { get; private set; }

        public DateOnly BirthDate { get; private set; }

        public MedicalCategory MedicalCategory { get; private set; }

        public CitizenStatus Status { get; private set; }

        public bool HasCriminalRecord { get; private set; }

        public bool IsStudent { get; private set; }

        public Death? Death { get; private set; }

        public bool IsAlive => Status != CitizenStatus.Deceased;

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Main constructor for creating a newborn citizen.
        /// </summary>
        public Citizen(
            string firstName,
            string lastName)
        {
            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;

            Age = 0;
            BirthDate = DateOnly.FromDateTime(DateTime.Now);

            MedicalCategory = GetRandomMedicalCategory();
            Status = CitizenStatus.Newborn;
            HasCriminalRecord = false;
            IsStudent = false;
        }

        /// <summary>
        /// Constructor for creating a specific citizen.
        /// </summary>
        public Citizen(
            string firstName,
            string lastName,
            int age,
            MedicalCategory medicalCategory,
            CitizenStatus status,
            bool hasCriminalRecord,
            bool isStudent)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            BirthDate = GetBirthDate(age);
            MedicalCategory = medicalCategory;
            Status = status;
            HasCriminalRecord = hasCriminalRecord;
            IsStudent = isStudent;
        }

        /// <summary>
        /// For testing and seeding purposes, this constructor allows for the creation of a citizen with a specific ID.
        /// </summary>
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
            return IsAlive
                && Age >= 18
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

        /// <summary>
        /// Advances the citizen's age by one year and raises a coming-of-age event
        /// the moment they cross into adulthood.
        /// </summary>
        public void HaveBirthday()
        {
            if (!IsAlive)
                return;

            var wasMinor = Age < AdultAge;

            Age++;

            if (wasMinor && Age >= AdultAge)
            {
                Status = CitizenStatus.WaitingForDraft;
                RaiseDomainEvent(new CitizenBecameAdultDomainEvent(Id));
            }

            if (Age > 27 && Status == CitizenStatus.WaitingForDraft)
                Status = CitizenStatus.Retired;
        }

        /// <summary>
        /// Marks the citizen as dead. Every death must carry a reason so the
        /// simulation never ends a life without explanation.
        /// </summary>
        public void Die(DeathReason reason, DateTimeOffset occurredAt)
        {
            if (!IsAlive)
                return;

            Death = ValueObjects.Death.Create(reason, occurredAt);
            Status = CitizenStatus.Deceased;

            RaiseDomainEvent(new CitizenDiedDomainEvent(Id, Death));
        }

        #region Helpers
        private MedicalCategory GetRandomMedicalCategory()
        {
            // Fit 95%
            // Limited Fit 4.7%
            // Permanently Unfit 0.3%
            var randomValue = Random.Shared.NextDouble();

            if (randomValue < 0.95)
                return MedicalCategory.Fit;
            else if (randomValue < 0.997)
                return MedicalCategory.LimitedFit;

            return MedicalCategory.PermanentlyUnfit;
        }

        private DateOnly GetBirthDate(int age)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return today.AddYears(-age);
        }
        #endregion
    }
}
