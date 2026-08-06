using MilitaryDraftSystem.Domain.Common;

namespace MilitaryDraftSystem.Domain.Entities
{
    /// <summary>
    /// Represents an autonomous automatic recruitment agent.
    /// Many agents may operate independently, each periodically searching for
    /// citizens that satisfy military requirements and drafting them automatically.
    /// Agents never create citizens.
    /// </summary>
    public sealed class AutomaticRecruitmentAgent : Entity<Guid>
    {
        public bool Enabled { get; private set; }

        public TimeSpan ExecutionInterval { get; private set; }

        public DateTimeOffset? LastExecutionAt { get; private set; }

        public void MarkExecuted(DateTimeOffset executedAt)
        {
            // Record the last successful execution time.
            LastExecutionAt = executedAt;
        }

        public void Disable()
        {
            Enabled = false;
        }

        public void Enable()
        {
            Enabled = true;
        }
    }
}
