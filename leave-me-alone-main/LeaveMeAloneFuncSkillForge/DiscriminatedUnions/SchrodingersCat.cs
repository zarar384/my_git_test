namespace LeaveMeAloneFuncSkillForge.DiscriminatedUnions
{
    /// <summary>
    /// Simple demo for illustrate Discriminated Union type.
    /// Represents Schrödinger Cat experiment with two possible states: Alive or Dead.
    /// </summary>
    public abstract record SchrodingersCat
    {
        public record Alive() : SchrodingersCat;
        public record Dead() : SchrodingersCat;
    }
}
