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

        public Gender Gender { get; private set; }

        /// <summary>
        /// The age, in years, at which this citizen is destined to die of old
        /// age if nothing else kills them first. Assigned once, at creation
        /// time, from a random statistical distribution that accounts for the
        /// difference in life expectancy between men and women. A value of
        /// zero means the lifespan has not been assigned yet (e.g. legacy
        /// data), in which case old-age death never triggers for this citizen.
        /// </summary>
        public int NaturalLifespanYears { get; private set; }

        public bool IsAlive => Status != CitizenStatus.Deceased;

        public string FullName => $"{FirstName} {LastName}";

        /// <summary>
        /// Main constructor for creating a newborn citizen.
        /// The medical category and gender are supplied by the caller since
        /// Domain must not perform its own randomization; probability
        /// decisions belong to Application.
        /// </summary>
        public Citizen(
            string firstName,
            string lastName,
            MedicalCategory medicalCategory,
            Gender gender = Enums.Gender.Male)
        {
            Id = Guid.NewGuid();

            FirstName = firstName;
            LastName = lastName;

            Age = 0;
            BirthDate = DateOnly.FromDateTime(DateTime.Now);

            MedicalCategory = medicalCategory;
            Status = CitizenStatus.Newborn;
            HasCriminalRecord = false;
            IsStudent = false;
            Gender = gender;
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
            bool isStudent,
            Gender gender = Enums.Gender.Male)
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
            Gender = gender;
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
            bool isStudent,
            Gender gender = Enums.Gender.Male)
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
            Gender = gender;
        }

        /// <summary>
        /// Assigns this citizen's natural lifespan. Must be computed by
        /// Application using the centralized <c>IRandomProvider</c>, since
        /// Domain never performs its own randomization. Can only be assigned
        /// once.
        /// </summary>
        public void AssignNaturalLifespan(int years)
        {
            if (years <= 0)
                throw new ArgumentOutOfRangeException(nameof(years), "Natural lifespan must be positive.");

            if (NaturalLifespanYears != 0)
                return;

            NaturalLifespanYears = years;
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
        /// Whether this citizen has reached or passed their assigned natural
        /// lifespan. A citizen whose lifespan was never assigned (zero) can
        /// never die of old age.
        /// </summary>
        public bool HasReachedNaturalLifespan()
        {
            return NaturalLifespanYears > 0 && Age >= NaturalLifespanYears;
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
        private DateOnly GetBirthDate(int age)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            return today.AddYears(-age);
        }
        #endregion
    }
}
