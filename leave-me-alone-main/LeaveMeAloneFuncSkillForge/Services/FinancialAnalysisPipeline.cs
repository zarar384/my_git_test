using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Functional.Monads;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public static class FinancialAnalysisPipeline
    {
        // Memoized pure function
        private static readonly Func<Transaction, decimal> RiskAdjustedAmount =
            ((Func<Transaction, decimal>)(t =>
                t.Amount * (t.Time.Hour < 9 ? 1.1m : 1.0m)
            )).Memorize(t => t.Id.ToString());

        // Transducer 
        private static readonly Func<IEnumerable<Transaction>, decimal> TotalAmount =
            ((Func<IEnumerable<Transaction>, IEnumerable<decimal>>)
                (txs => txs.Select(RiskAdjustedAmount)))
            .ToTransducer(xs => xs.Sum());

        public static StateMaybe<decimal, IReadOnlyList<string>> Analyze(
            IEnumerable<Transaction> transactions,
            IEnumerable<TaskData> tasks,
            IEnumerable<Film> films)
        {
            return 0m.ToStateMaybe(transactions)

                // Validate input
                .Bind<decimal, IEnumerable<Transaction>, IEnumerable<Transaction>>(
                    (state, txs) =>
                        txs.Any()
                            ? new Something<IEnumerable<Transaction>>(txs)
                            : new Nothing<IEnumerable<Transaction>>())

                // Calculate transaction total
                .Bind((state, txs) =>
                        new Something<decimal>(TotalAmount(txs)))

                // Adjust by tasks
                .Bind((state, total) =>
                        new Something<decimal>(
                            tasks.Aggregate(
                                total,
                                (acc, t) => acc - t.EstimatedHours)))

                // Add film revenue
                .Bind((state, total) =>
                        new Something<decimal>(
                            films.Aggregate(
                                total,
                                (acc, f) => acc + (decimal)f.BoxOfficeRevenue)))

                 // Project to report
                .Bind((state, final) =>
                        new Something<IReadOnlyList<string>>(
                            new List<string>
                            {
                                $"Final score: {final}",
                                $"Transactions: {transactions.Count()}",
                                $"Tasks: {tasks.Count()}",
                                $"Films: {films.Count()}"
                            }));

        }
    }
}
