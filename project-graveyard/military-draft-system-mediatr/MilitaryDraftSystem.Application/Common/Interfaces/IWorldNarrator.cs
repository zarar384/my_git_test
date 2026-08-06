using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Application.Common.Interfaces
{
    /// <summary>
    /// Produces human-readable narration of meaningful simulation events.
    /// Complements structured logging with output that makes the world feel alive.
    /// </summary>
    public interface IWorldNarrator
    {
        void CitizenBorn(Citizen citizen);

        void CitizenBecameAdult(Citizen citizen);

        void CitizenDrafted(Citizen citizen, Guid? officerId, Guid? agentId);

        void SummonsCreated(Summons summons);

        void CitizenDied(Citizen citizen, Death death);
    }
}
