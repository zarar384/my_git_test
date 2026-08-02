using MediatR;

namespace MilitaryDraftSystem.Application.Population.Commands.RunPopulationGeneration
{
    /// <summary>
    /// Executes the automatic population generation process.
    /// </summary>
    public sealed record RunPopulationGenerationCommand : IRequest;
}
