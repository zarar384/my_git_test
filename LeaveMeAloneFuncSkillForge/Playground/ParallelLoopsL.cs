using System.Collections.Concurrent;

namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class ParallelLoopsL
    {
        private static IEnumerable<int> Range(int start, int end, int step = 1)
        {
            for (int i = start; i < end; i += step)
            {
                yield return i;
            }
        }

        public static void Run()
        {
            RunPartitionerSquareEachValueDemo();
        }

        // chanked to use Partitioner for better performance on large datasets
        public static void RunPartitionerSquareEachValueDemo()
        {
            const int count = 1000000;
            var numbers = Enumerable.Range(1, count).ToArray();
            var squaredNumbers = new int[count];

            // Using Partitioner to divide the work into chunks
            Parallel.ForEach(Partitioner.Create(0, numbers.Length), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    squaredNumbers[i] = numbers[i] * numbers[i];
                }

               // Console.WriteLine($"Processed range {range.Item1} to {range.Item2}");
            });

            Console.WriteLine("All numbers have been squared.");
        }
        
        public static void RunLocalStateDemo()
        {
            var sum = 0;
            Parallel.For(0, 10,
                () => 0, // init local state
                (i, state, localSum) =>  
                {
                    localSum += i;
                    Console.WriteLine($"Iteration {i}, Local Sum: {localSum}");
                    return localSum;
                },
                localSum => 
                {
                    Interlocked.Add(ref sum, localSum);
                    Console.WriteLine($"Final Local Sum: {localSum}");
                });

            Console.WriteLine($"Total Sum: {sum}");
        }

        public static void RunStupLoopsDemo()
        {
            try
            {
                DemoLoops();
            }
            catch (AggregateException ae)
            {
                ae.Handle(ex =>
                {
                    Console.WriteLine($"Handled exception: {ex.Message}");
                    return true;
                });
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine($"Operation was canceled: {oce.Message}");
            }
        }

        private static void DemoLoops()
        {
            var cts = new System.Threading.CancellationTokenSource();
            ParallelOptions po = new ParallelOptions
            {
                CancellationToken = cts.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            var p = Parallel.For(0, 20, po, (int i, ParallelLoopState state) =>
            {
                Console.WriteLine($"Iteration {i}[{Task.CurrentId}]");

                if(i==10)
                {
                   //throw new Exception("Demo exception at iteration 10");
                    //state.Stop(); // Stops as soon as possible - doesnt stop immediately
                    // state.Break();
                    cts.Cancel(); 
                }
            });

            Console.WriteLine($"Loop completed: {p.IsCompleted}");
            if (p.LowestBreakIteration.HasValue)
            {
                Console.WriteLine($"Lowest break iteration: {p.LowestBreakIteration}");
            }
        }

        public static void RunParallelDemo()
        {
            int item = 42;

            var action1 = new Action(() =>
            {
                Console.WriteLine($"Processing item {item} on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                System.Threading.Thread.Sleep(100);
            });

            var action2 = new Action(() =>
            {
                Console.WriteLine($"Handling item {item} on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                System.Threading.Thread.Sleep(150);
            });

            var action3 = new Action(() =>
            {
                Console.WriteLine($"Finalizing item {item} on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                System.Threading.Thread.Sleep(200);
            });

            Parallel.Invoke(action1, action2, action3);

            Parallel.For(0, 5, i =>
            {
                Console.WriteLine($"Parallel loop iteration {i} on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                System.Threading.Thread.Sleep(50);
            });

            string[] people = { "Alice", "Bob", "Charlie", "Diana" };
            Parallel.ForEach(people, person =>
            {
                Console.WriteLine($"Hello, {person}! (thread {System.Threading.Thread.CurrentThread.ManagedThreadId})");
                System.Threading.Thread.Sleep(75);
            });

            int[] numbers = { 1, 2, 3, 4, 5 };
            var po = new ParallelOptions { MaxDegreeOfParallelism = 2 };
            Parallel.ForEach(numbers, po, number =>
            {
                int square = number * number;
                Console.WriteLine($"The square of {number} is {square} (thread {System.Threading.Thread.CurrentThread.ManagedThreadId})");
                System.Threading.Thread.Sleep(60);
            });

            Parallel.ForEach(Range(1, 6), number =>
            {
                int cube = number * number * number;
                Console.WriteLine($"The cube of {number} is {cube} (thread {System.Threading.Thread.CurrentThread.ManagedThreadId})");
                System.Threading.Thread.Sleep(80);
            });
        }
    }
}
