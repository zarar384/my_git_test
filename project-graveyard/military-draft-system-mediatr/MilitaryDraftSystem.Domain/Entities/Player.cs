using MilitaryDraftSystem.Domain.Common;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents a real player controlling one or more recruitment officers.
    /// The current game only has a single player, but the model must not assume
    /// that: multiple players will eventually be able to see each other's
    /// officers and play simultaneously.
    /// </summary>
    public sealed class Player : Entity<Guid>
    {
        public string DisplayName { get; private set; } = null!;

        public DateTimeOffset CreatedAt { get; private set; }

        public Player(Guid id, string displayName, DateTimeOffset createdAt)
        {
            Id = id;
            DisplayName = displayName;
            CreatedAt = createdAt;
        }

        private Player()
        {
            // Required by EF Core.
        }
    }
}
