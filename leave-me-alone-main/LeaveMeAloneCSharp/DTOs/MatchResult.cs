namespace LeaveMeAloneCSharp.DTOs
{
    public record MatchResult(
        SPS MyMove,
        SPS OpponentMove,
        GameResult Result,
        string Reason
    );
}
