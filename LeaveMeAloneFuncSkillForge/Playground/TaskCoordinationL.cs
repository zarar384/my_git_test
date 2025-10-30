namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class TaskCoordinationL
    {
        public static void Run()
        {
            RunChildTaskWithContinuationDemo();
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
