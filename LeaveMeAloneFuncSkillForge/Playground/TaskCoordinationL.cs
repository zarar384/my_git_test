namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class TaskCoordinationL
    {
        // SemaphoreSlim to limit concurrent access to a resource
        // allows up to 2 concurrent tasks and a maximum of 10 waiting tasks
        private static SemaphoreSlim semaphore = new SemaphoreSlim(2, 10);

        // CountdownEvent to wait for multiple tasks to complete
        private static int taskCount = 5;
        static CountdownEvent countdown = new CountdownEvent(taskCount);
        private static Random random = new Random();

        // Barrier to synchronize two tasks
        // two: snack and eat
        static Barrier barrier = new Barrier(2, b =>
        {
            Console.WriteLine($"Phase {b.CurrentPhaseNumber}. Both participants have finished eating. Proceeding to dessert...");
        });

        public static void Run()
        {
            RunSemaphoreSlimDemo();
        }

        public static void RunSemaphoreSlimDemo()
        {
            for (int i = 0; i < 10; i++)
            {
                int taskId = i;
                Task.Factory.StartNew(async () =>
                {
                    Console.WriteLine($"Task {taskId} is waiting to enter the semaphore...");
                    await semaphore.WaitAsync(); // wait to enter the semaphore, only 2 tasks can enter at the same time
                    try
                    {
                        Console.WriteLine($"Task {taskId} has entered the semaphore.");
                        await Task.Delay(random.Next(1000, 3000)); // work
                        Console.WriteLine($"Task {taskId} is leaving the semaphore.");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            // Wait for user input to exit
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            semaphore.Dispose();
        }

        public static void RunManualResetEventDemo()
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task is waiting for the signal..."); // 1
                manualResetEvent.WaitOne(); // wait for the signal
                Console.WriteLine("Task received the signal and is proceeding."); // 4
            });

            var finalTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Press Enter to signal the task to proceed.");  // 0
                Console.ReadLine(); // 2
                manualResetEvent.Set(); // signal the waiting task
                Console.WriteLine("Signal sent."); // 3
            });

            finalTask.Wait();
        }

        // AutoResetEvent automatically resets to non-signaled after releasing a single waiting thread,
        // so only one waiting task will proceed each time Set() is called.
        public static void RunAutoResetEventDemo()
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task is waiting for the signal..."); // 1
                autoResetEvent.WaitOne(); // wait for the signal
                Console.WriteLine("Task received the signal and is proceeding."); // 4
                var ok =  autoResetEvent.WaitOne(1000); // wait for the signal again
                if(ok) Console.WriteLine("Task received the signal again and is proceeding."); // does not happen
                else Console.WriteLine("Task did not receive the signal again."); 
            });
            var tasl = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Press Enter to signal the task to proceed.");  // 0
                Console.ReadLine(); // 2
                autoResetEvent.Set(); // signal the waiting task
                Console.WriteLine("Signal sent."); // 3
            });
            tasl.Wait();
        }

        public static void RunCountdownEventDemo()
        {
            for (int i = 0; i < taskCount; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Task {Task.CurrentId} is working...");
                    Thread.Sleep(random.Next(500, 2000)); 
                    Console.WriteLine($"Task {Task.CurrentId} completed.");
                    countdown.Signal(); // signal that this task is done
                });
            }

            var finalTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Final task is waiting for all tasks to complete...");
                countdown.Wait(); 
                Console.WriteLine("All tasks completed. Final task proceeding.");
            });

            finalTask.Wait();
        }

        public static void RunBarrierDemo()
        {
            var snack = Task.Factory.StartNew(() => Snack());
            var eat = Task.Factory.StartNew(() => Eat());

            var dessert = Task.Factory.ContinueWhenAll(new[] {snack, eat} , t =>
            {
                Console.WriteLine("Enjoying dessert!");
            });

            dessert.Wait();
        }

        private static void Snack()
        {
            Console.WriteLine("Snack time!");
            Thread.Sleep(1000);
            barrier.SignalAndWait();  // 0 faze - 0 step
            Console.WriteLine("Finished cheeps");
            barrier.SignalAndWait(); // 1  - 0
            Console.WriteLine("Finished crackers");

        }

        private static void Eat()
        {
            Console.WriteLine("Eating...");
            Thread.Sleep(2000);
            barrier.SignalAndWait(); // 0 faze - 1 step
            Console.WriteLine("Finished one snack");
            barrier.SignalAndWait();  // 1 - 1
            Console.WriteLine("Finished second snack");
        }


        public static void RunChildTaskDemo()
        {
            var parent = new Task(() =>
            {
                var child1 = new Task(() =>
                {
                    Console.WriteLine("Child 1 starting.");
                    Task.Delay(2000).Wait();
                    Console.WriteLine("Child 1 completed.");
                }, TaskCreationOptions.AttachedToParent);
                var child2 = new Task(() =>
                {
                    Console.WriteLine("Child 2 starting.");
                    Task.Delay(1000).Wait();
                    Console.WriteLine("Child 2 completed.");
                }, TaskCreationOptions.AttachedToParent);
                child1.Start();
                child2.Start();
            });

            parent.Start();

            try
            {
                parent.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine($"Exception: {e.Message}");
                    return true;
                });
            }
        }

        public static void RunChildTaskWithContinuationDemo()
        {
            var parent = new Task(() =>
            {
                var child = new Task(() =>
                {
                    Console.WriteLine($"Child task {Task.CurrentId} starting.");
                    Task.Delay(1000).Wait();
                    Console.WriteLine($"Child task {Task.CurrentId} completed.");
                }, TaskCreationOptions.AttachedToParent);
                var continuation = child.ContinueWith(t =>
                {
                    Console.WriteLine($"Continuation task starting after child {t.Id} completion.");
                    throw new InvalidOperationException("Simulated exception in continuation.");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnRanToCompletion);
                var failureContinuation = continuation.ContinueWith(t =>
                {
                    Console.WriteLine($"Child task {t.Id} failed with exception: {t.Exception?.GetBaseException().Message}");
                }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.OnlyOnFaulted);
                child.Start();
            });
            parent.Start();

            try
            {
                parent.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    Console.WriteLine($"Exception: {e.Message}");
                    return true;
                });
            }
        }

        public static void RunTaskContinueWithDemo()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Task started.");
            });

            var task2 = task.ContinueWith(t =>
            {
                Console.WriteLine($"Task {t.Id} completed.");
            });

            task2.Wait();
        }

        public static void RunTaskContinueWithDemo2()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Task {Task.CurrentId} started.");
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"Task {Task.CurrentId} started.");
            });

            var task3 = Task.Factory.ContinueWhenAny(new[] { task, task2 },
                t =>
                {
                    Console.WriteLine("One of the tasks has completed.");
                    Console.WriteLine($"Completed Task Id: {t.Id}");
                });

            task3.Wait();
        }
    }
}
