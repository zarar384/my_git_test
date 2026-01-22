using System.Collections.Concurrent;

namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class ConcurrentCollectionsL
    {
        private static ConcurrentDictionary<string, string> _animeCharacters =
            new ConcurrentDictionary<string, string>();

        private static BlockingCollection<int> _blockingCollection =
            new BlockingCollection<int>(new ConcurrentBag<int>(), boundedCapacity: 10);

        static private CancellationTokenSource _cts = new CancellationTokenSource();
        static private Random random = new Random();

        public static void Run()
        {
            ProducerConsumerExample();
        }

        // Consumer method to take items from the BlockingCollection
        private static void RunConsumer()
        {
            foreach (var item in _blockingCollection.GetConsumingEnumerable())
            {
                _cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"\tConsumed: {item}");
                Thread.Sleep(random.Next(2000));
            }
        }

        // Producer method to add items to the BlockingCollection
        private static void RunProducer()
        {
            while (true)
            {
                _cts.Token.ThrowIfCancellationRequested();

                int i = random.Next(100);
                _blockingCollection.Add(i);
                Console.WriteLine($"Produced: {i}");
                Thread.Sleep(random.Next(1000));
            }
        }

        private static void AddLuffy()
        {
            bool added = _animeCharacters.TryAdd("One Piece", "Monkey D. Luffy");
            string who = Task.CurrentId.HasValue ? $"Task {Task.CurrentId}" : "Main Thread";
            Console.WriteLine($"{who} {(added ? "added" : "did not add")} Luffy");
        }

        public static void ConcurrentDictionaryExample()
        {
            Task.Factory.StartNew(AddLuffy).Wait();
            AddLuffy();

            _animeCharacters["Naruto"] = "Uzumaki Naruto";
            _animeCharacters.AddOrUpdate("Naruto", "Sasuke Uchiha",
                (key, existingValue) => existingValue + "--> Sasuke Uchiha");

            Console.WriteLine($"Naruto character: {_animeCharacters["Naruto"]}");

            _animeCharacters["Bleach"] = "Kurosaki Ichigo";
            var bleachCharacter = _animeCharacters.GetOrAdd("Bleach", "Abarai Renji");

            Console.WriteLine($"Bleach character: {bleachCharacter}");

            const string keyToRemove = "One Piece";
            bool didRemove = _animeCharacters.TryRemove(keyToRemove, out var removedCharacter);

            if (didRemove)
            {
                Console.WriteLine($"Successfully removed {keyToRemove} - {removedCharacter}");
            }
            else
            {
                Console.WriteLine($"Failed to remove {keyToRemove}  - {removedCharacter}");
            }

            foreach (var kvp in _animeCharacters)
            {
                Console.WriteLine($"Anime: {kvp.Key}, Character: {kvp.Value}");
            }
        }

        public static void ConcurrentQueueExample()
        {
            // Queue FIFO
            var q = new ConcurrentQueue<string>();
            // Enqueue adds items to the end of the queue
            q.Enqueue("First");
            q.Enqueue("Second");

            // Second, First - because it's a queue (FIFO)

            string result;
            // TryDequeue removes the item from the queue
            if (q.TryDequeue(out result))
            {
                Console.WriteLine($"Dequeued: {result}");
            }

            // Now only "Second" remains in the queue
            // TryPeek does not remove the item
            if (q.TryPeek(out result))
            {
                Console.WriteLine($"Peeked: {result}");
            }
        }

        public static void ConcurrentStackExample()
        {
            // Stack LIFO
            var stack = new ConcurrentStack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            stack.Push(4);

            int result;

            // TryPeek does not remove the item
            if (stack.TryPeek(out result))
            {
                Console.WriteLine($"Peeked: {result}");
            }

            // TryPop removes the item from the stack
            if (stack.TryPop(out result))
            {
                Console.WriteLine($"Popped: {result}");
            }

            var items = new int[5];
            // TryPopRange removes multiple items from the stack
            // Here try to pop up to 5 items, but there are only 3 left
            // So it will pop 3 items
            // other 2 items in the array will remain default(int) which is 0
            if (stack.TryPopRange(items, 0, 5) > 0)
            {
                Console.WriteLine($"Popped Range: {string.Join(", ", items.Select(i => i))}");
            }
        }

        public static void ConcurrentBagExample()
        {
            // Bag - unordered collection
            var concurrentBag = new ConcurrentBag<int>();
            var tasks = new List<Task>();

            // Start 10 tasks to add and take items from the ConcurrentBag
            // Each task adds its index to the bag and then tries to take an item
            for (int i = 0; i < 10; i++)
            {
                int localI = i;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    concurrentBag.Add(localI);
                    Console.WriteLine($"Task {Task.CurrentId} added {localI}");

                    int result;
                    // TryPeek does not remove the item
                    if (concurrentBag.TryPeek(out result))
                    {
                        Console.WriteLine($"Task {Task.CurrentId} took {result}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            int last;
            // TryTake removes an item from the bag
            if (concurrentBag.TryTake(out last))
            {
                Console.WriteLine($"Last item taken from bag: {last}");
            }
        }

        public static void ProducerConsumerExample()
        {
            var consumerTask = Task.Factory.StartNew(RunConsumer, _cts.Token);
            var producerTask = Task.Factory.StartNew(RunProducer, _cts.Token);
            // Let the producer and consumer run for 10 seconds
            Task.Delay(10000).Wait();
            // Signal cancellation to stop producer and consumer
            _cts.Cancel();
            try
            {
                Task.WaitAll(producerTask, consumerTask);
            }
            catch (AggregateException ae)
            {
                foreach (var ex in ae.InnerExceptions)
                {
                    if (ex is OperationCanceledException)
                    {
                        Console.WriteLine("Operation was canceled.");
                    }
                    else
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                }
            }
            finally
            {
                _blockingCollection.CompleteAdding();
            }
        }
    }
}
