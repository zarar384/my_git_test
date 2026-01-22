using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace LeaveMeAloneFuncSkillForge.Playground
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

        // Time-based operators are one of Rx's strongest features
        public static void RxTimeOperatorsDemo()
        {
            var subscription = Observable.Interval(TimeSpan.FromMilliseconds(300))
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(x => Console.WriteLine($"Throttled: {x}"));

            Thread.Sleep(5000);
            subscription.Dispose();
        }
    }
}
