using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public static class TournamentRunner
    {
        public static TournamentResult Run(
            string nameA,
            Strategy strategyA,
            string nameB,
            Strategy strategyB,
            int rounds,
            Func<SPS, SPS, MatchResult> resolve,
            ISPSMatchServiceWrapper? matchService = null)
        {
            var results = (matchService != null)
                    ? matchService.PlayGames(strategyA, strategyB, rounds, resolve).ToList()
                    : SPSMatchService.PlayGames(strategyA, strategyB, rounds, resolve).ToList();

            int winsA = results.Count(r => r.Result == GameResult.Win);
            int winsB = results.Count(r => r.Result == GameResult.Lose);
            int draws = results.Count(r => r.Result == GameResult.Draw);

            return new TournamentResult(nameA, nameB, winsA, winsB, draws, (double)winsA / rounds, (double)winsB / rounds, results);
        }

        public static void RunAllAgainstAll(int rounds = 1000)
        {
            var strategies = new Dictionary<string, Strategy>()
            {
                ["AlwaysScissors"] = Stategies.AlwaysScissors!,
                ["MirrorLast"] = Stategies.MirrorLastMove!,
                ["CounterLast"] = Stategies.CounterLastMove!
            };

            foreach (var (nameA, strategyA) in strategies)
            {
                foreach (var (nameB, strategyB) in strategies)
                {
                    if (nameA == nameB) continue;

                    var result = Run(nameA, strategyA, nameB, strategyB, rounds, SPSMatchFunc.CalculateMatchResult);
                    Console.WriteLine($"{nameA} vs {nameB}: {result.WinRateA:P2} - {result.WinRateB:P2} (Draws: {(double)result.Draws / rounds:P2})");
                }
            }
        }
    }
}