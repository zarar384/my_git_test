using LeaveMeAloneCSharp.Models;
using System.Collections.Concurrent;

namespace LeaveMeAloneCSharp.Playground
{
    public static class ParallelLoopsL
    {
        public static async Task Run()
        {
          await  RunParallelToAsyncDemo();
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

        public static void RunParallelAggregationDemo()
        {
            var numbers = Enumerable.Range(1, 1_000_000);

            int total = 0;

            Parallel.ForEach(
                numbers,
                () => 0, // local init
                (n, state, localSum) =>
                {
                    return localSum + n;
                },
                localSum =>
                {
                    Interlocked.Add(ref total, localSum);
                });

            Console.WriteLine($"Parallel sum: {total}");
        }

        public static void RunBreakVsStopDemo()
        {
            Parallel.For(
                0, 
                100, 
                new ParallelOptions { MaxDegreeOfParallelism = 3 }, 
                (i, state) =>
            {
                Thread.Sleep(200); // Simulate work

                Console.WriteLine($"Iteration {i}");

                if (i == 5)
                {
                    state.Break(); // try Stop() and compare
                }
            });
        }

        public static void RunParallelInvokeCpuDemo()
        {
            Parallel.Invoke(
                () => HeavyCalculation("A"),
                () => HeavyCalculation("B"),
                () => HeavyCalculation("C")
            );
        }

        private static void HeavyCalculation(string name)
        {
            Console.WriteLine($"Started {name} on thread {Thread.CurrentThread.ManagedThreadId}");
            double result = 0;
            for (int i = 0; i < 10_000_000; i++)
                result += Math.Sqrt(i);
            Console.WriteLine($"Finished {name}");
        }

        public static void RunSecurityLogAnalysisDemo()
        {
            const int logCount = 2_000_000;

            var random = new Random();

            var logs = Enumerable.Range(0, logCount)
                                 .Select(i => new SecurityLog
                                 {
                                     UserId = random.Next(1, 1000),
                                     Action = random.Next(1, 4),
                                     ResourceId = random.Next(1, 5000),
                                     Timestamp = DateTime.UtcNow.AddSeconds(-random.Next(0, 100000)),
                                     Payload = Guid.NewGuid().ToString()
                                 })
                                 .ToArray();

            Console.WriteLine("Starting security log analysis...");

            int suspiciousCount = 0; 

            var suspiciousLogs = new ConcurrentBag<SecurityLog>();

            Parallel.ForEach(
                Partitioner.Create(0, logs.Length), // Create partitions for better performance on large datasets
                () =>  new LocalStats(), // Initialize local state for each partition
                (range, state, local) =>
                {
                    // Simulate complex analysis for each log in the assigned range
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var log = logs[i];

                        double riskScore = CalculateRiskScore(log);

                        if(riskScore > 0.92)
                        {
                            local.SuspiciousCount++;
                            suspiciousLogs.Add(log);
                        }

                        local.Processed++;
                        local.RiskSum += riskScore;
                    }

                    return local;
                },
                // Aggregate results from each partition
                local => 
                {
                    // Aggregate suspicious count
                    Interlocked.Add(ref suspiciousCount, local.SuspiciousCount); 

                    // Log local stats for this partition
                    if (local.Processed > 0)
                    {
                        Console.WriteLine($"Processed {local.Processed} logs with average risk {local.RiskSum / local.Processed:F4}");
                    }
                    else
                    {
                        Console.WriteLine("No logs processed in this partition.");
                    }
                });

            Console.WriteLine($"Total suspicious logs: {suspiciousCount}");
            Console.WriteLine($"Collected {suspiciousLogs.Count} suspicious logs for further analysis.");
        }

        public static void RunDynamicParallelFileScannerDemo()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Console.WriteLine($"Scanning root: {root}");

            long totalSize = 0;
            int fileCount = 0;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            Task rootTask = Task.Factory.StartNew(
                () => TraverseDirectory(root),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);

            rootTask.Wait();

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Scan finished");
            Console.WriteLine($"Files found: {fileCount}");
            Console.WriteLine($"Total size: {totalSize / 1024 / 1024} MB");
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms");


            void TraverseDirectory(string path)
            {
                try
                {
                    // process files in current directory
                    foreach (var file in Directory.GetFiles(path))
                    {
                        try
                        {
                            var info = new FileInfo(file);

                            Interlocked.Add(ref totalSize, info.Length);
                            Interlocked.Increment(ref fileCount);
                        }
                        catch
                        {
                            // ignore file access errors
                        }
                    }

                    // spawn tasks for subdirectories
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        Task.Factory.StartNew(
                            () => TraverseDirectory(dir),
                            CancellationToken.None,
                            TaskCreationOptions.AttachedToParent,
                            TaskScheduler.Default);
                    }
                }
                catch
                {
                    // ignore directory access errors
                }
            }
        }

        public static void RunParallelInvokeImagePipelineDemo()
        {
            const int pixelCount = 4_000_000;

            var random = new Random();

            // simulate grayscale image
            var pixels = new byte[pixelCount];

            random.NextBytes(pixels);

            Console.WriteLine($"Image loaded with {pixels.Length} pixels");

            int histogramScore = 0;
            int edgeScore = 0;
            int noiseScore = 0;
            int compressionScore = 0;

            var sw = System.Diagnostics.Stopwatch.StartNew();

            Parallel.Invoke(

                // histogram analysis
                () =>
                {
                    int local = 0;

                    int[] histogram = new int[256];

                    foreach (var p in pixels)
                        histogram[p]++;

                    for (int i = 0; i < histogram.Length; i++)
                    {
                        for (int k = 0; k < 200; k++)
                            local += (int)Math.Sqrt(histogram[i] + k);
                    }

                    Interlocked.Add(ref histogramScore, local);

                    Console.WriteLine($"Histogram analysis finished on thread {Thread.CurrentThread.ManagedThreadId}");
                },

                // Edge detection simulation
                () =>
                {
                    int local = 0;

                    for (int i = 1; i < pixels.Length - 1; i++)
                    {
                        int diff = Math.Abs(pixels[i - 1] - pixels[i + 1]);

                        for (int k = 0; k < 100; k++)
                            local += (int)Math.Sqrt(diff + k);
                    }

                    Interlocked.Add(ref edgeScore, local);

                    Console.WriteLine($"Edge detection finished on thread {Thread.CurrentThread.ManagedThreadId}");
                },

                // noise estimation
                () =>
                {
                    int local = 0;

                    for (int i = 2; i < pixels.Length - 2; i++)
                    {
                        int variance =
                            Math.Abs(pixels[i] - pixels[i - 1]) +
                            Math.Abs(pixels[i] - pixels[i + 1]);

                        for (int k = 0; k < 120; k++)
                            local += (int)Math.Log(variance + k + 1);
                    }

                    Interlocked.Add(ref noiseScore, local);

                    Console.WriteLine($"Noise analysis finished on thread {Thread.CurrentThread.ManagedThreadId}");
                },

                // compression complexity estimation
                () =>
                {
                    int local = 0;

                    foreach (var p in pixels)
                    {
                        int value = p;

                        for (int k = 0; k < 150; k++)
                            local += (int)Math.Sin(value + k);
                    }

                    Interlocked.Add(ref compressionScore, local);

                    Console.WriteLine($"Compression estimation finished on thread {Thread.CurrentThread.ManagedThreadId}");
                }

            );

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Pipeline summary:");
            Console.WriteLine($"Histogram score: {histogramScore}");
            Console.WriteLine($"Edge score: {edgeScore}");
            Console.WriteLine($"Noise score: {noiseScore}");
            Console.WriteLine($"Compression score: {compressionScore}");
            Console.WriteLine($"Processing time: {sw.ElapsedMilliseconds} ms");
        }

        public static async Task RunParallelToAsyncDemo()
        {
            Console.WriteLine("Starting parallel to async demo...");
            Console.WriteLine();


            var numbers = Enumerable.Range(1, 5);

            Console.WriteLine("Starting parallel work WITHOUT blocking caller thread...");
            Console.WriteLine($"Running on thread {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine();

            // use Task.Run to offload the parallel work to a background thread, allowing the caller thread to remain responsive
            // forexample, in a UI app this would prevent freezing the UI while the work is being done
            await Task.Run(() =>
            {
                Parallel.ForEach(numbers, number =>
                {
                    Console.WriteLine($"Processing {number} on thread {Thread.CurrentThread.ManagedThreadId}");
                    Thread.Sleep(300); // Simulate work
                });
            });

            Console.WriteLine();
            Console.WriteLine("All work completed, back to caller thread.");

            Console.WriteLine();
            Console.WriteLine("End of demo.");

        }

        #region helpers
        // Simulate a complex risk score calculation based on log properties
        private static double CalculateRiskScore(SecurityLog log)
        {
            double score = 0;

            for(int i =0; i<200; i++)
            {
                // Simulate complex calculations
                score += Math.Sqrt(log.UserId * i + log.ResourceId);
                score += Math.Sin(log.Action + i);
                score += Math.Log(i + 1);
            }

            score = Math.Abs(score % 1); // Normalize to [0,1]

            return score;
        }

        private static IEnumerable<int> Range(int start, int end, int step = 1)
        {
            for (int i = start; i < end; i += step)
            {
                yield return i;
            }
        }
        #endregion  
    }
}
