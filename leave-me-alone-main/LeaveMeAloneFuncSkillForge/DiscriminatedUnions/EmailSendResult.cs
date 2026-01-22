namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    public abstract record EmailSendResult
    {
        public record EmailSuccess : EmailSendResult;
        public record EmailFailure(Exception Error) : EmailSendResult;
    }
}
