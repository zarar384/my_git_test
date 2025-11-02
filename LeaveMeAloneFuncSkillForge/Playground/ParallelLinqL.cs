namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class ParallelLinqL
    {
        public static void Run()
        {
            RunParallelEnumerableWithCustomAggregateDemo();
        }

        public static void RunParallelMergeOptionsDemo()
        {
            var numbers = Enumerable.Range(1, 20).ToArray();
            var results = numbers.AsParallel()
                .WithMergeOptions(ParallelMergeOptions.NotBuffered)
                .Select(x =>
                {
                    var result = Math.Log10(x);
                    Console.WriteLine($"Processed {x}, Result: {result}, TaskID: {Task.CurrentId}");
                    return result;
                });

            foreach (var result in results)
            {
                Console.WriteLine($"Final Result: {result}");
            }
        }

        public static void RunParallelEnumerableWithCustomAggregateDemo()
        {
            var numbers = ParallelEnumerable.Range(1, 1000);
            var sumOfSquares = numbers.AsParallel()
                                      .Aggregate(
                                          0,
                                          (subtotal, n) => subtotal + n * n,
                                          (total1, total2) => total1 + total2,
                                          finalResult => finalResult);

            Console.WriteLine($"The sum of squares from 1 to 1000 is: {sumOfSquares}");
        }

        public static void RunAsParallelDemo()
        {
            var numbers = Enumerable.Range(1, 1000000);
            var squaredNumbers = numbers.AsParallel()
                                        .Select(n => n * n)
                                        .ToArray();

            Console.WriteLine("All numbers have been squared using PLINQ.");
            Console.WriteLine($"First 5 squared numbers: {string.Join(", ", squaredNumbers.Take(5))}");
        }

        public static void RunParallelQueryWithDegreeOfParallelismDemo()
        {
            var numbers = Enumerable.Range(1, 1000000);
            var squaredNumbers = numbers.AsParallel()
                                        .WithDegreeOfParallelism(4) // Limit to 4 concurrent tasks
                                        .Select(n => n * n)
                                        .ToArray();

            Console.WriteLine("All numbers have been squared with limited parallelism using PLINQ.");
            Console.WriteLine($"First 5 squared numbers: {string.Join(", ", squaredNumbers.Take(5))}");
        }

        public static void RunAsParallelDemo2()
        {
            const int itemCount = 20;

            var items = Enumerable.Range(1, itemCount).ToArray();
            var results = new int[itemCount];

            items.AsParallel().ForAll(item =>
            {
                var newValue = item * item * item;
                Console.WriteLine($"Item: {item}, Cubed: {newValue}, TaskID {Task.CurrentId}");
                results[item - 1] = newValue;
            });

            Console.WriteLine();
            Console.WriteLine();

            var qubes = results.AsParallel().AsOrdered().Take(10).ToArray();
            //var qubes = results.Take(10);

            foreach (var result in qubes)
            {
                Console.WriteLine($"{result}\t");
            }
            Console.WriteLine();
        }

        public static void RunParallelQueryWithCancellationDemo()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var numbers = Enumerable.Range(1, 5).ToArray();

            var results = new int[numbers.Count()];
            try
            {
                numbers.AsParallel()
                       .AsOrdered()
                       .Select((n, index) => (Value: n * n, Index: index))
                       .WithCancellation(token)
                       .ForAll(x =>
                       {
                           if (x.Value == 100)
                               cts.Cancel();

                           results[x.Index] = x.Value; 
                       });
            }
            catch(AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine($"{e.GetType().Name}: {e.Message}");
                    return true;
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The PLINQ operation was canceled.");
            }
            finally
            {
                Console.WriteLine($"First 5 squared numbers: {string.Join(", ", results.Take(5))}");
            }
        }
    }
}
