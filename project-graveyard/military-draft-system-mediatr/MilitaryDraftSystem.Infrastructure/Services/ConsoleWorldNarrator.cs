using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Infrastructure.Services
{
    /// <summary>
    /// Writes lively, human-readable narration of simulation events through
    /// the standard logging abstraction (Debug level), so it can be routed,
    /// filtered or silenced via configuration like any other log source,
    /// instead of writing to the console directly.
    /// </summary>
    public sealed class ConsoleWorldNarrator(ILogger<ConsoleWorldNarrator> logger) : IWorldNarrator
    {
        public void CitizenBorn(Citizen citizen)
        {
            logger.LogDebug("God created citizen {CitizenId} ({CitizenName}).", citizen.Id, citizen.FullName);
        }

        public void CitizenBecameAdult(Citizen citizen)
        {
            logger.LogDebug("Citizen {CitizenId} ({CitizenName}) turned {Age}.", citizen.Id, citizen.FullName, citizen.Age);
        }

        public void CitizenDrafted(Citizen citizen, Guid? officerId, Guid? agentId)
        {
            if (agentId is not null)
            {
                logger.LogDebug(
                    "Automatic agent {AgentId} drafted citizen {CitizenId} ({CitizenName}).",
                    agentId,
                    citizen.Id,
                    citizen.FullName);
            }
            else
            {
                logger.LogDebug(
                    "Recruitment officer {OfficerId} drafted citizen {CitizenId} ({CitizenName}).",
                    officerId,
                    citizen.Id,
                    citizen.FullName);
            }
        }

        public void SummonsCreated(Summons summons)
        {
            logger.LogDebug("Summons {SummonsId} created.", summons.Id);
        }

        public void CitizenDied(Citizen citizen, Death death)
        {
            logger.LogDebug(
                "Citizen {CitizenId} ({CitizenName}) died. Cause of death: {CauseOfDeath}",
                citizen.Id,
                citizen.FullName,
                death.Describe());
        }
    }
}
