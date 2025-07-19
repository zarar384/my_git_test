
namespace LeaveMeAloneFuncSkillForge
{
    public static class App
    {
        public static void RunApp()
        {
            var result = SPSMatchService.PlayGames(
                myStrategy: Stategies.CounterLastMove,
                theirStraategy: Stategies.MirrorLastMove,
                rounds: 5,
                resolve: SPSMatchFunc.CalculateMatchResult
                );

            Console.WriteLine(SPSMatchFunc.FormatHistory(result));
        }
    }
}
