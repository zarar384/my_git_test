using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace LeaveMeAloneCSharp.Playground
{
    public static class ReactiveL
    {
        public static void Run()
        {
            RxBasicDemo();
            //RxHotVsColdDemo();
            //RxSubjectDemo();
            //RxErrorHandlingDemo();
            //RxTimeOperatorsDemo();
        }

        // Basic Rx example: push-based stream with LINQ operators
        public static void RxBasicDemo()
        {
            var subscription = Observable.Interval(TimeSpan.FromSeconds(1)) // emits a long value every second
                .Where(x => x % 2 == 0)
                .Select(x => $"Even tick: {x}")
                .Subscribe(
                    x => Console.WriteLine(x),
                    ex => Console.WriteLine($"Error: {ex.Message}"),
                    () => Console.WriteLine("Completed")
                );

            Thread.Sleep(5000);
            subscription.Dispose();
        }

        // Demonstrates cold vs hot observables
        public static void RxHotVsColdDemo()
        {
            Console.WriteLine("Cold observable:");
            var cold = Observable.Interval(TimeSpan.FromSeconds(1));

            // Each subscription starts its own sequence
            var sub1 = cold.Subscribe(x => Console.WriteLine($"Cold A: {x}"));
            Thread.Sleep(3000);

            var sub2 = cold.Subscribe(x => Console.WriteLine($"Cold B: {x}"));
            Thread.Sleep(3000);

            sub1.Dispose();
            sub2.Dispose();

            Console.WriteLine("\nHot observable:");
            var hot = Observable.Interval(TimeSpan.FromSeconds(1)).Publish();

            // Starts emitting values regardless of subscriptions
            hot.Connect();

            var hotSub1 = hot.Subscribe(x => Console.WriteLine($"Hot A: {x}"));
            Thread.Sleep(3000);

            var hotSub2 = hot.Subscribe(x => Console.WriteLine($"Hot B: {x}"));
            Thread.Sleep(3000);

            hotSub1.Dispose();
            hotSub2.Dispose();
        }

        // Subject allows manual pushing of events to observers
        public static void RxSubjectDemo()
        {
            var subject = new Subject<int>();

            var subscription = subject
                .Where(x => x > 10)
                .Subscribe(x => Console.WriteLine($"Received: {x}"));

            subject.OnNext(5);   // ignored by filter
            subject.OnNext(15);  // passed to observer
            subject.OnNext(25);  // passed to observer

            subject.OnCompleted();
            subscription.Dispose();
        }

        // Demonstrates error propagation in Rx streams
        public static void RxErrorHandlingDemo()
        {
            var observable = Observable.Create<int>(observer =>
                {
                    observer.OnNext(1);
                    observer.OnNext(2);
                    observer.OnError(new InvalidOperationException("Something went wrong"));
                    observer.OnNext(3); // never executed
                    return () => { };
                });

            observable.Subscribe(
                x => Console.WriteLine($"Value: {x}"),
                ex => Console.WriteLine($"Error: {ex.Message}"),
                () => Console.WriteLine("Completed")
            );
        }

        // Time-based operators are one of Rx strongest features
        public static void RxTimeOperatorsDemo()
        {
            var subscription = Observable.Interval(TimeSpan.FromMilliseconds(300))
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(x => Console.WriteLine($"Throttled: {x}"));

            Thread.Sleep(5000);
            subscription.Dispose();
        }

        // Converts .NET event into Rx observable stream
        public static void RxEventToObservableDemo()
        {
            var timer = new System.Timers.Timer(500);
            timer.Start();

            var ticks = Observable.FromEventPattern<System.Timers.ElapsedEventHandler, System.Timers.ElapsedEventArgs>(
                handler => (s, e) => handler(s, e),
                handler => timer.Elapsed += handler,
                handler => timer.Elapsed -= handler);

            var subscription = ticks
                .Select(e => e.EventArgs.SignalTime)
                .Subscribe(time => Console.WriteLine($"Tick at {time:HH:mm:ss.fff}"));

            Thread.Sleep(3000);
            subscription.Dispose();
            timer.Stop();
        }

        // Demonstrates switching execution context
        public static void RxObserveOnDemo()
        {
            var uiContext = SynchronizationContext.Current ?? new SynchronizationContext();

            var subscription = Observable.Interval(TimeSpan.FromMilliseconds(500))
                .Do(x => Console.WriteLine($"Produced {x} on thread {Thread.CurrentThread.ManagedThreadId}"))
                .ObserveOn(uiContext)
                .Subscribe(x =>
                    Console.WriteLine($"Observed {x} on thread {Thread.CurrentThread.ManagedThreadId}")
                );

            Thread.Sleep(3000);
            subscription.Dispose();
        }

        // Groups incoming events into batches
        public static void RxBufferDemo()
        {
            var subscription = Observable.Interval(TimeSpan.FromMilliseconds(400))
                .Buffer(3)
                .Subscribe(batch =>
                {
                    Console.WriteLine($"Batch received: {string.Join(", ", batch)}");
                });

            Thread.Sleep(5000);
            subscription.Dispose();
        }

        // Window creates streams of streams
        public static void RxWindowDemo()
        {
            var subscription = Observable.Interval(TimeSpan.FromMilliseconds(400))
                .Window(3)
                .Subscribe(window =>
                {
                    Console.WriteLine("New window");

                    window.Subscribe(
                        x => Console.WriteLine($"Window item: {x}"),
                        () => Console.WriteLine("Window completed")
                    );
                });

            Thread.Sleep(5000);
            subscription.Dispose();
        }

        // Demonstrates event flow control
        public static void RxThrottleVsSampleDemo()
        {
            var source = new Subject<int>();

            var throttleSub = source
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(x => Console.WriteLine($"Throttle: {x}"));

            var sampleSub = source
                .Sample(TimeSpan.FromMilliseconds(500))
                .Subscribe(x => Console.WriteLine($"Sample: {x}"));

            // simulate fast event stream
            for (int i = 0; i < 20; i++)
            {
                source.OnNext(i);
                Thread.Sleep(100);
            }

            Thread.Sleep(2000);

            throttleSub.Dispose();
            sampleSub.Dispose();
        }

        // Simulates reactive autocomplete search pipeline
        public static void RxReactiveSearchDemo()
        {
            var input = new Subject<string>();

            var subscription = input
                .Throttle(TimeSpan.FromMilliseconds(500)) // wait until typing pauses
                .DistinctUntilChanged() // ignore same queries
                .Do(q => Console.WriteLine($"Searching for: {q}"))
                .SelectMany(query => FakeSearchApi(query).ToObservable()) // async search
                .Subscribe(
                    results =>
                    {
                        Console.WriteLine($"Results: {string.Join(", ", results)}");
                    },
                    ex => Console.WriteLine($"Error: {ex.Message}")
                );

            // simulate typing
            input.OnNext("r");
            Thread.Sleep(100);

            input.OnNext("re");
            Thread.Sleep(100);

            input.OnNext("rea");
            Thread.Sleep(700); // triggers search

            input.OnNext("react");
            Thread.Sleep(700); // triggers another search

            input.OnCompleted();
            subscription.Dispose();
        }

        // Simulated async search request
        private static async Task<List<string>> FakeSearchApi(string query)
        {
            await Task.Delay(300); // simulate network

            return new List<string>
            {
                $"{query}_result1",
                $"{query}_result2",
                $"{query}_result3"
            };
        }
    }
}
