namespace LeaveMeAloneFuncSkillForge.DTOs
{
    public record class TournamentResult(
        string StrategyA,
        string StrategyB,
        int WinsA,
        int WinsB,
        int Draws,
        double WinRateA,
        double WinRateB,
        IReadOnlyList<MatchResult> Rounds
        );
}
