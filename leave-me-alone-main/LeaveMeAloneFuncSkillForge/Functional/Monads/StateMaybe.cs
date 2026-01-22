using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;

namespace LeaveMeAloneFuncSkillForge.Functional.Monads
{
    public class StateMaybe<TS, TV>
    {
        public TS CurrentState { get; init; }
        public Maybe<TV> CurrentValue { get; init; }

        public StateMaybe(TS state, TV value)
        {
            CurrentState = state;
            CurrentValue = new Something<TV>(value);
        }

        public StateMaybe(TS state, Maybe<TV> value)
        {
            CurrentState = state;
            CurrentValue = value;
        }
    }
}
