using MediatR;

namespace MilitaryDraftSystem.Application.Population.Queries.GetCemeteryRecords
{
    /// <summary>
    /// Lists the cemetery: every citizen and recruitment officer who has died,
    /// with who they were, when and how they died, and whether their original
    /// record still exists in the population table.
    /// </summary>
    public sealed record GetCemeteryRecordsQuery : IRequest<IReadOnlyList<CemeteryRecordDto>>;
}
