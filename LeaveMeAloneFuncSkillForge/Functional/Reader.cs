namespace LeaveMeAloneFuncSkillForge.Functional
{
    public sealed class Reader<TEnv, TResult>
    {
        private readonly Func<TEnv, TResult> _func;

        public Reader(Func<TEnv, TResult> func)
        {
            _func = func;
        }

        public TResult Run(TEnv env) => _func(env);

        public Reader<TEnv, TNext> Bind<TNext>
            (Func<TResult, Reader<TEnv, TNext>> binder)
        {
            return new Reader<TEnv, TNext>(env =>
            {
                var result = _func(env);
                var nextReader = binder(result);
                return nextReader.Run(env);
            });
        }
    }
}
