using MediatR;

namespace MilitaryDraftSystem.Application.Population.Commands.PurgeDeceasedCitizens
{
    /// <summary>
    /// Physically removes deceased citizens from the population table once
    /// enough time has passed since their death, keeping the living-population
    /// table lean. Their history is preserved separately via cemetery records
    /// and death statistics, which are never deleted.
    /// </summary>
    public sealed record PurgeDeceasedCitizensCommand : IRequest;
}
