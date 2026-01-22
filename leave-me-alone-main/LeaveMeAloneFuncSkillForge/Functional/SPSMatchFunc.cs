namespace LeaveMeAloneFuncSkillForge.Functional
{
    public static class SPSMatchFunc
    {
        public static Func<SPS, SPS, MatchResult> CalculateMatchResult = (me, them) =>
        (me, them) switch
        {
            _ when me == them => new MatchResult(me, them, GameResult.Draw, "Same move"),
            (SPS.Scissor, SPS.Paper) => new MatchResult(me, them, GameResult.Win, "Scissors cut Paper"),
            (SPS.Paper, SPS.Stone) => new MatchResult(me, them, GameResult.Win, "Paper wraps Stone"),
            (SPS.Stone, SPS.Scissor) => new MatchResult(me, them, GameResult.Win, "Stone crushes Scissors"),
            _ => new MatchResult(me, them, GameResult.Lose, $"{them} beats {me}")
        };

        public static string FormatHistory(IEnumerable<MatchResult> games) =>
            string.Join("\n", games.Select((g, i) =>
                $"Game {i + 1}: Me={g.MyMove}, Them={g.OpponentMove} => {g.Result} ({g.Reason})"));
    }
}
