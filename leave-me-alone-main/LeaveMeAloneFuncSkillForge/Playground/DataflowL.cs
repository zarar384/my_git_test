using System.Threading.Tasks.Dataflow;

namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class DataflowL
    {
        public static async Task Run()
        {
            await SimplePipelineDemo();
            //await ParallelBlockDemo();
            //await ErrorPropagationDemo();
            //await BoundedCapacityDemo();
        }

        // Simple pipeline: input -> transform -> action
        public static async Task SimplePipelineDemo()
        {
            // Transforms input data (like LINQ Select)
            var multiplyBlock = new TransformBlock<int, int>(x =>
            {
                Console.WriteLine($"Multiply {x}");
                return x * 2;
            });

            // Final action block (terminal stage)
            var printBlock = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Result: {x}");
            });

            // Link blocks together and propagate completion
            multiplyBlock.LinkTo(printBlock, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            // Post data into the pipeline
            for (int i = 1; i <= 5; i++)
            {
                multiplyBlock.Post(i);
            }

            // Signal that no more data will come
            multiplyBlock.Complete();

            // Wait for the pipeline to finish
            await printBlock.Completion;
        }

        // Parallel processing inside a block
        public static async Task ParallelBlockDemo()
        {
            var block = new TransformBlock<int, int>(
                async x =>
                {
                    Console.WriteLine($"Processing {x} on thread {Thread.CurrentThread.ManagedThreadId}");
                    await Task.Delay(500);
                    return x * x;
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 3
                });

            var printer = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Squared: {x}");
            });

            block.LinkTo(printer, new DataflowLinkOptions { PropagateCompletion = true });

            for (int i = 1; i <= 6; i++)
            {
                block.Post(i);
            }

            block.Complete();
            await printer.Completion;
        }

        // Error propagation through the pipeline
        public static async Task ErrorPropagationDemo()
        {
            var block = new TransformBlock<int, int>(x =>
            {
                if (x == 3)
                    throw new InvalidOperationException("Boom");

                return x * 10;
            });

            var printer = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Value: {x}");
            });

            block.LinkTo(printer, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            try
            {
                block.Post(1);
                block.Post(2);
                block.Post(3); // will cause exception
                block.Post(4);

                block.Complete();
                await printer.Completion;
            }
            catch (AggregateException ex)
            {
                var flat = ex.Flatten();
                Console.WriteLine($"Caught error: {flat.InnerException?.Message}");
            }
        }

        // Bounded capacity to control buffering
        public static async Task BoundedCapacityDemo()
        {
            var slowBlock = new ActionBlock<int>(
                x =>
                {
                    Console.WriteLine($"Processing {x}");
                    Thread.Sleep(1000);
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 2
                });

            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine($"Posting {i}");
                await slowBlock.SendAsync(i); // waits if buffer is full
            }

            slowBlock.Complete();
            await slowBlock.Completion;
        }
    }
}
