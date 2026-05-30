using LeaveMeAloneCSharp.Interfaces;

namespace LeaveMeAloneCSharp.Services
{
    public class SPSMatchServiceWrapper : ISPSMatchServiceWrapper
    {
        public IEnumerable<MatchResult> PlayGames(Strategy a, Strategy b, int rounds, Func<SPS, SPS, MatchResult> resolve)
        {
            return SPSMatchService.PlayGames(a, b, rounds, resolve);
        }
    }
}
