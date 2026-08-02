using FluentValidation;

namespace MilitaryDraftSystem.Application.Population.Commands.RunPopulationGeneration
{
    /// <summary>
    /// Validates the automatic population generation command.
    /// </summary>
    public sealed class RunPopulationGenerationCommandValidator
        : AbstractValidator<RunPopulationGenerationCommand>
    {
    }
}
