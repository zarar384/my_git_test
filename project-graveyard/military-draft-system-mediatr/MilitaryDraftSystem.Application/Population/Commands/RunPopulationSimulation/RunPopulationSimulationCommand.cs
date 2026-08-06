using MediatR;

namespace MilitaryDraftSystem.Application.Population.Commands.RunPopulationSimulation
{
    /// <summary>
    /// Executes one tick of the world simulation: aging citizens, birthdays and deaths.
    /// </summary>
    public sealed record RunPopulationSimulationCommand : IRequest;
}
