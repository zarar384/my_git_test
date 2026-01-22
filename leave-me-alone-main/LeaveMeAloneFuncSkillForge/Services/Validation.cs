using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
namespace LeaveMeAloneFuncSkillForge.Services
{
    public static class Validation
    {
        public static Either<ErrorInfo, string> ValidateGenre(string genre) =>
            string.IsNullOrWhiteSpace(genre)
            ? new Left<ErrorInfo, string>(new ErrorInfo("InvalidGenre", "Genre cannot be empty"))
            : new Right<ErrorInfo, string>(genre);

        public static Either<ErrorInfo, int> ValidateTopN(int n) =>
            n <= 0
            ? new Left<ErrorInfo, int>(new ErrorInfo("InvalidTopN", "Top N must be positive"))
            : new Right<ErrorInfo, int>(n);
    }
}
