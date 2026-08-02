using MilitaryDraftSystem.Domain.Entities;

namespace MilitaryDraftSystem.Application.Population.Services.Interfaces
{
    /// <summary>
    /// Generates citizens according to the population generator configuration.
    /// </summary>
    public interface IPopulationGenerationService
    {
        IReadOnlyCollection<Citizen> Generate(PopulationGenerator generator);
    }
}
