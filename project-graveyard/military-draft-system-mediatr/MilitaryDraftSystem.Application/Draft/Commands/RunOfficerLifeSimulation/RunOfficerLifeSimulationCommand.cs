using MediatR;

namespace MilitaryDraftSystem.Application.Draft.Commands.RunOfficerLifeSimulation
{
    /// <summary>
    /// Simulates the passage of time for every active recruitment officer:
    /// resignation, accidental death, suicide, or death on duty may occur.
    /// </summary>
    public sealed record RunOfficerLifeSimulationCommand : IRequest;
}
