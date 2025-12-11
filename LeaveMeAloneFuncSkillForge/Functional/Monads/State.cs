namespace LeaveMeAloneFuncSkillForge.Functional.Monads
{
    public class State<TS, TV>
    {
        public TS CurrentState { get; init; }
        public TV CurrentValue { get; init; }

        public State(TS s, TV v)
        {
            CurrentState = s;
            CurrentValue = v;
        }
    }
}
