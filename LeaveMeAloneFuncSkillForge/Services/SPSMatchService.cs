namespace LeaveMeAloneFuncSkillForge.Services
{
    public delegate SPS Strategy(IEnumerable<MatchResult> history);

    public class Stategies
    {
        public static Strategy AlwaysScissors => _ => SPS.Scissor;

        public static Strategy MirrorLastMove => history =>
            history.LastOrDefault()?.MyMove ?? SPS.Stone;

        public static Strategy CounterLastMove => history =>
            history.LastOrDefault()?.MyMove switch
            {
                SPS.Scissor => SPS.Stone,
                SPS.Stone => SPS.Paper,
                SPS.Paper => SPS.Scissor,
                _ => SPS.Stone,
            };
    }

    public class SPSMatchService
    {
        // ты тупой? это 'камень, ножницы, бумага' - ЭТО КЛАССИКА! ЭТО ЗНАТЬ НАДО!
        public static IEnumerable<MatchResult> PlayGames(
            Strategy myStrategy,
            Strategy theirStraategy,
            int rounds,
            Func<SPS, SPS, MatchResult> resolve
            )
        { 
            var history = new List<MatchResult>();

            for (int i = 0; i < rounds; i++)
            {
                var me = myStrategy(history);
                var them = theirStraategy(history);
                var result = resolve(me, them);
                history.Add(result);
            }

            return history;
        }
    }
}
