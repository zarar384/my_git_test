namespace LeaveMeAloneFuncSkillForge.Interfaces
{
    public interface ISPSMatchServiceWrapper
    {
        IEnumerable<MatchResult> PlayGames(Strategy a, Strategy b, int rounds, Func<SPS, SPS, MatchResult> resolve);
    }
}
