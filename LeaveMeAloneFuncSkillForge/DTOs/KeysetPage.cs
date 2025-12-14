namespace LeaveMeAloneFuncSkillForge.DTOs
{
    public sealed record KeysetPage<T, TKey>(
        IReadOnlyList<T> Items,
        bool HasNextPage,
        TKey? NextCursor
    )
    where TKey : struct;
}
