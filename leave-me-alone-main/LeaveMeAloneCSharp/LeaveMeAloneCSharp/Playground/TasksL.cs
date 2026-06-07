namespace LeaveMeAloneCSharp.Playground
{
    public static class TasksL
    {
        public static async Task Run()
        {
            await TestThreadPoolSchedulingAsync();
        }

        private static void RunTaskExceptionDemo()
        {
            /*
             DemoTaskExceptionHandling output:
                Handled InvalidOperationException from t: Something went wrong in Task
             Main - caught unhandled AggregateException:
              - AccessViolationException from t2: Access violation in Task
             Main - done
            */
            try
            {
                DemoTaskExceptionHandling();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Main - caught unhandled AggregateException:");
                foreach (var inner in ex.InnerExceptions)
                {
                    Console.WriteLine($" - {inner.GetType().Name} from {inner.Source}: {inner.Message}");
                }
            }

            Console.WriteLine("Main - done");
        }

        private static void DemoTaskExceptionHandling()
        {
            var t = Task.Factory.StartNew(() =>
            {
                throw new InvalidOperationException("Something went wrong in Task") { Source = "t" };
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                throw new AccessViolationException("Access violation in Task") { Source = "t2" };
            });

            try
            {
                Task.WaitAll(new[] { t, t2 });
            }
            catch (AggregateException ae)
            {
                //Console.WriteLine("Caught AggregateException:");
                //foreach (var ex in ae.InnerExceptions)
                //{
                //    Console.WriteLine($" - {ex.GetType().Name} from {ex.Source}: {ex.Message}");
                //}
                ae.Handle(e =>
                {
                    switch (e)
                    {
                        case InvalidOperationException ioe:
                            Console.WriteLine($"Handled InvalidOperationException from {ioe.Source}: {ioe.Message}");
                            return true;
                        default:
                            return false; // unhandled
                    }
                });
            }
        }

        private static void Run1()
        {
            // async
            Task.Factory.StartNew(() => Write('1'));

            var t = new Task(() => Write('2'));
            t.Start();

            // main thread
            Write('3');
        }

        private static void Run2()
        {
            // async
            Task t = new Task(Write, "Hello from Task");
            t.Start();

            Task.Factory.StartNew(Write, 12345);
        }

        private static void Run3()
        {
            // async
            string text1 = "testing", text2 = "1234567890";
            var task1 = new Task<int>(TextLength, text1);
            task1.Start();

            Task<int> task2 = Task.Factory.StartNew(TextLength, text2);

            // wait for tasks to complete
            Console.WriteLine($"Length of '{text1}' is {task1.Result}");
            Console.WriteLine($"Length of '{text2}' is {task2.Result}");
        }

        private static void RunWithToken1()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            // register a callback on cancellation
            token.Register(() =>
            {
                Console.WriteLine("Cancellation requested - cleanup here");
            });

            var t = new Task(() =>
            {
                int i = 0;
                while (true)
                {
                    //if(token.IsCancellationRequested)
                    //{
                    //    // hard exit of a task - canonical way
                    //    throw new OperationCanceledException(token);

                    //    // soft exit of a task
                    //    //Console.WriteLine("Task - cancellation requested, exiting");
                    //    //break;
                    //}

                    // another canonical way
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine($"{i++}\t");
                }
            }, token);
            t.Start();

            Task.Factory.StartNew(() =>
            {
                token.WaitHandle.WaitOne();
                Console.WriteLine("Task 2 - detected cancellation, exiting");
            });

            Console.ReadKey();
            cts.Cancel();
        }

        private static void RunWithToken2()
        {
            var planned = new CancellationTokenSource();
            var preventative = new CancellationTokenSource();
            var emergency = new CancellationTokenSource();

            // if any of the tokens is cancelled, the linked token is cancelled
            var paranoid = CancellationTokenSource.CreateLinkedTokenSource(
                planned.Token, preventative.Token, emergency.Token);

            // register callbacks
            planned.Token.Register(() => Console.WriteLine("Planned cancelled"));
            preventative.Token.Register(() => Console.WriteLine("Preventative cancelled"));
            emergency.Token.Register(() => Console.WriteLine("Emergency cancelled"));
            paranoid.Token.Register(() => Console.WriteLine("Paranoid cancelled"));

            Task.Factory.StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    paranoid.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"Paranoid task running {i++}");
                    Thread.Sleep(1000);
                }
            }, paranoid.Token);

            Console.ReadKey();
            emergency.Cancel();
        }

        private static void RunWaitInsideTask()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var t = new Task(() =>
            {
                Console.WriteLine("Press any key. You have 5 sec.");
                bool cancalled = token.WaitHandle.WaitOne(5000);

                Console.WriteLine(cancalled
                    ? "Task - detected cancellation, exiting"
                    : "Task - timeout expired, exiting");

                //SpinWait.SpinUntil(() => false, 2000);
            }, token);
            t.Start();

            Console.ReadKey();
            cts.Cancel();
        }

        private static void RunWaitAll() 
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var t = new Task(() =>
            {
                Console.WriteLine("Task - starting");

                for (int i = 0; i < 5; i++)
                {
                    token.ThrowIfCancellationRequested();
                    Console.WriteLine($"Task - iteration {i}");
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Task - completed");
            }, token);
            t.Start();

            Task t2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task 2 - waiting for cancellation");
                Thread.Sleep(2500);
            }, token);

            Task.Run(() =>
            {
                Console.WriteLine("Task 3 - detected cancellation, exiting");
                Console.ReadKey();
                cts.Cancel();
            });

            Task.WaitAll(new[] { t, t2 }, 4000, token);

            Console.WriteLine("Task t status: " + t.Status);
            Console.WriteLine("Task t2 status: " + t2.Status);
        }

        private static void RunDynamicTaskParallelismDemo()
        {
            var results = new int[5];
            Task parent = Task.Factory.StartNew(() =>
            {
                var tasks = new List<Task>();

                for (int i = 0; i < 5; i++)
                {
                    int local = i;
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine($"Child {local} on thread {Thread.CurrentThread.ManagedThreadId}");
                        Thread.Sleep(200);
                        results[local] = local;
                    }, TaskCreationOptions.AttachedToParent));
                }
            });

            parent.Wait();
            Console.WriteLine("All child tasks completed");

            Console.WriteLine("Results: " + string.Join(", ", results));
        }

        private static async Task TestThreadPoolSchedulingAsync()
        {
            // offload blocking file scan to thread pool - keep caller free
            Task<long> scanTask = Task.Run(() =>
            {
                long total = 0;
                foreach (var _ in Enumerable.Range(0, 1_000_000))
                    total++;
                return total;
            });

            // async lambda - returns Task<T>, not just Task
            Task<string> reportTask = Task.Run(async () =>
            {
                await Task.Delay(30); // simulate fetching report metadata
                return "report-2024-q4.pdf";
            });

            await Task.WhenAll(scanTask, reportTask);

            Console.WriteLine($"[THREAD POOL] scanned {scanTask.Result} items");
            Console.WriteLine($"[THREAD POOL] report: {reportTask.Result}");
            Console.WriteLine();
        }

        #region helpers
        private static void Write(char c)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(c);
            }
        }

        private static void Write(object o)
        {
            int i = 1000;
            while (i-- > 0)
            {
                Console.Write(o);
            }
        }

        private static int TextLength(object o)
        {
            Console.WriteLine($"Task id {Task.CurrentId} processing obj {o}");
            return o.ToString().Length;
        }
        #endregion
    }
}
