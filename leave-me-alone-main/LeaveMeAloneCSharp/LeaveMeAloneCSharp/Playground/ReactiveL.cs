using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace LeaveMeAloneCSharp.Playground
{
    public static class ReactiveL
    {
        public static async Task Run()
        {
             TaskToObservableDemo();
            //RxHotVsColdDemo();
            //RxSubjectDemo();
            //RxErrorHandlingDemo();
            //RxTimeOperatorsDemo();
        }

        // Basic Rx example: push-based stream with LINQ operators
        public static void RxBasicDemo()
        {
            var subscription = RxBasicStream() // emits a long value every second
                .Subscribe(
                    x => Console.WriteLine(x),
                    ex => Console.WriteLine($"Error: {ex.Message}"),
                    () => Console.WriteLine("Completed")
                );

            Thread.Sleep(5000);
            subscription.Dispose();
        }

        public static IObservable<string> RxBasicStream(IScheduler scheduler = null)
        {
            return Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
                .Where(x => x % 2 == 0)
                .Select(x => $"Even tick: {x}");
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

            var subscription = RxSubjectStream(subject)
                .Subscribe(x => Console.WriteLine($"Received: {x}"));

            subject.OnNext(5);   // ignored by filter
            subject.OnNext(15);  // passed to observer
            subject.OnNext(25);  // passed to observer

            subject.OnCompleted();
            subscription.Dispose();
        }

        public static IObservable<int> RxSubjectStream(IObservable<int> source)
        {
            return source.Where(x => x > 10);
        }

        // Demonstrates error propagation in Rx streams
        public static void RxErrorHandlingDemo()
        {
            var observable = RxErrorStream();

            observable.Subscribe(
                x => Console.WriteLine($"Value: {x}"),
                ex => Console.WriteLine($"Error: {ex.Message}"),
                () => Console.WriteLine("Completed")
            );
        }

        public static IObservable<int> RxErrorStream()
        {
            return Observable.Create<int>(observer =>
            {
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnError(new InvalidOperationException("Something went wrong"));
                observer.OnNext(3); // never executed
                return () => { };
            });
        }


        // Time-based operators are one of Rx strongest features
        public static void RxTimeOperatorsDemo()
        {
            var subscription = RxThrottleStream()
                .Subscribe(x => Console.WriteLine($"Throttled: {x}"));

            Thread.Sleep(5000);
            subscription.Dispose();
        }

        public static IObservable<long> RxThrottleStream(IScheduler scheduler = null)
        {
            return Observable.Interval(TimeSpan.FromMilliseconds(300), scheduler)
                .Throttle(TimeSpan.FromSeconds(1), scheduler);
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

            var subscription = RxSearchStream(input, FakeSearchApi, withInfo: true)
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

        public static IObservable<List<string>> RxSearchStream(
            IObservable<string> input,
            Func<string, Task<List<string>>> search,
            bool withInfo = false)
        {
            return input
                .Throttle(TimeSpan.FromMilliseconds(500))   // wait until typing pauses
                .DistinctUntilChanged()                     // ignore same queries
                .Do(q =>
                {
                    if (withInfo)
                        Console.WriteLine($"Searching for: {q}");
                })
                .SelectMany(q => search(q).ToObservable()); // async search
        }

        public static async Task ObservableToAwaitDemo()
        {
            Console.WriteLine("OBSERVABLE TO AWAIT DEMO");
            Console.WriteLine();

            // Emits values every 0.1 seconds (0,1,2,3,4)
            // This is a COLD observable:
            // each subscription creates a new independent sequence (starts from 0 again)
            var observable = Observable
                .Interval(TimeSpan.FromSeconds(0.1))
                .Take(5);
            //.Publish().RefCount().Take(5); // Uncomment to make it HOT (shared sequence) 0... 5... 6, 7, 8, 9, 10

            Console.WriteLine("Starting FirstAsync (no waiting full stream)...");

            // FirstAsync takes the FIRST value from the stream and converts it to a Task
            int first = (int)await observable.FirstAsync();
            Console.WriteLine($"First element: {first}");

            Console.WriteLine();

            Console.WriteLine("Starting LastAsync (waits full stream)...");

            // LastAsync takes the LAST value from the stream and converts it to a Task
            int last = (int)await observable.LastAsync();
            Console.WriteLine($"Last element: {last}");

            Console.WriteLine();

            Console.WriteLine("Starting ToList (collect all)...");

            // ToList takes ALL values from the stream and converts them to a List
            var all = await observable.ToList();
            Console.WriteLine($"All elements: {string.Join(", ", all)}");

            Console.WriteLine();
        }

        public static void TaskToObservableDemo()
        {
            Console.WriteLine("TASK TO OBSERVABLE DEMO");
            Console.WriteLine();

            // Simulate an async operation that returns a value
            Task<string> asyncTask = Task.Run(async () =>
            {
                await Task.Delay(500);
                return "Hello from Task!";
            });

            // Convert the Task to an Observable
            var observable = asyncTask.ToObservable();

            // Subscribe to the Observable
            var subscription = observable.Subscribe(
                result => Console.WriteLine($"Received: {result}"),
                ex => Console.WriteLine($"Error: {ex.Message}"),
                () => Console.WriteLine("Observable task completed")
            );

            // Wait for the task to complete
            Thread.Sleep(1000); // simulate waiting for async operation

            subscription.Dispose();
        }

        public static async Task ObservableToAsyncStreamDemo()
        {
            Console.WriteLine("OBSERVABLE TO ASYNC STREAM (push to pull bridge)");
            Console.WriteLine();

            // Emits values every 0.2 seconds (0,1,2,3,4)
            var observable = Observable.Interval(TimeSpan.FromMilliseconds(200)).Take(5);

            Console.WriteLine("Consuming observable as async stream...");

            // ToAsyncEnumerable converts the Observable into an IAsyncEnumerable
            await foreach (var item in observable.ToAsyncEnumerable())
            {
                Console.WriteLine($"Pulled value: {item}");
            }

            Console.WriteLine("Stream completed");
            Console.WriteLine();
        }
    }
}
