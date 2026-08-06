using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Domain.Entities;
using MilitaryDraftSystem.Domain.ValueObjects;

namespace MilitaryDraftSystem.Infrastructure.Services
{
    /// <summary>
    /// Writes lively, human-readable narration of simulation events to the console.
    /// </summary>
    public sealed class ConsoleWorldNarrator : IWorldNarrator
    {
        public void CitizenBorn(Citizen citizen)
        {
            Console.WriteLine($"God created citizen {citizen.FullName}.");
        }

        public void CitizenBecameAdult(Citizen citizen)
        {
            Console.WriteLine($"Citizen {citizen.FullName} turned {citizen.Age}.");
        }

        public void CitizenDrafted(Citizen citizen, Guid? officerId, Guid? agentId)
        {
            Console.WriteLine(
                agentId is not null
                    ? $"Automatic Agent {agentId} drafted {citizen.FullName}."
                    : $"Recruitment Officer {officerId} drafted {citizen.FullName}.");
        }

        public void SummonsCreated(Summons summons)
        {
            Console.WriteLine($"Summons #{summons.Id} created.");
        }

        public void CitizenDied(Citizen citizen, Death death)
        {
            Console.WriteLine($"{citizen.FullName} died.");
            Console.WriteLine("Cause of death:");
            Console.WriteLine(death.Describe());
        }
    }
}
