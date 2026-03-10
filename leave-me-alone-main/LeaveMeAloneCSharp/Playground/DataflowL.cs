using LeaveMeAloneCSharp.Models;
using System.Threading.Tasks.Dataflow;

namespace LeaveMeAloneCSharp.Playground
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

        // linear dataflow pipeline
        public static async Task LinearPipelineDemo()
        {
            // st. 1: multiply
            var multiply = new TransformBlock<int, int>(x =>
            {
                Console.WriteLine($"Multiply {x}");
                return x * 2;
            });

            // st. 2: subtract
            var subtract = new TransformBlock<int, int>(x =>
            {
                Console.WriteLine($"Subtract 1 from {x}");
                return x - 1;
            });

            // Final st.
            var printer = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Final value: {x}");
            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            multiply.LinkTo(subtract, linkOptions);
            subtract.LinkTo(printer, linkOptions);

            for (int i = 1; i <= 5; i++)
                multiply.Post(i);

            multiply.Complete();

            await printer.Completion;
        }

        // routing with filters
        public static async Task BranchingPipelineDemo()
        {
            var source = new BufferBlock<int>();

            // even numbers handler
            var evenBlock = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Even handler got {x}");
            });

            // odd numbers handler
            var oddBlock = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Odd handler got {x}");
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            // route even numbers
            source.LinkTo(evenBlock, options, x => x % 2 == 0);

            // route odd numbers
            source.LinkTo(oddBlock, options);

            for (int i = 1; i <= 10; i++)
                source.Post(i);

            source.Complete();

            await Task.WhenAll(evenBlock.Completion, oddBlock.Completion);
        }

        // load balancing workers with bounded capacity
        public static async Task LoadBalancingPipelineDemo()
        {
            var source = new BufferBlock<int>();

            var workerOptions = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 1,
                MaxDegreeOfParallelism = 1
            };

            var workerA = new ActionBlock<int>(async x =>
            {
                Console.WriteLine($"Worker A processing {x}");
                await Task.Delay(500);
            }, workerOptions);

            var workerB = new ActionBlock<int>(async x =>
            {
                Console.WriteLine($"Worker B processing {x}");
                await Task.Delay(500);
            }, workerOptions);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            source.LinkTo(workerA, linkOptions);
            source.LinkTo(workerB, linkOptions);

            for (int i = 1; i <= 10; i++)
                source.Post(i);

            source.Complete();

            await Task.WhenAll(workerA.Completion, workerB.Completion);
        }

        // streaming log processing pipeline
        public static async Task LogProcessingPipelineDemo()
        {
            var random = new Random();

            // st. 1: parse raw logs
            var parseBlock = new TransformBlock<string, LogEntry>(line =>
            {
                Console.WriteLine($"Parsing log: {line}");

                return new LogEntry
                {
                    UserId = random.Next(1, 100),
                    Action = line,
                    Timestamp = DateTime.UtcNow
                };
            });

            // st. 2: validation
            var validateBlock = new TransformManyBlock<LogEntry, LogEntry>(entry =>
            {
                if (entry.UserId % 5 == 0)
                {
                    Console.WriteLine($"Rejected suspicious log from user {entry.UserId}");
                    return Enumerable.Empty<LogEntry>();
                }

                return new[] { entry };
            });

            // st. 3: transform
            var transformBlock = new TransformBlock<LogEntry, ProcessedLog>(entry =>
            {
                return new ProcessedLog
                {
                    UserId = entry.UserId,
                    Action = entry.Action.ToUpper(),
                    RiskScore = entry.UserId % 10
                };
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            });

            // st. 4: persistence
            var storageBlock = new ActionBlock<ProcessedLog>(log =>
            {
                Console.WriteLine($"Stored log for user {log.UserId} with risk {log.RiskScore}");
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            parseBlock.LinkTo(validateBlock, options);
            validateBlock.LinkTo(transformBlock, options);
            transformBlock.LinkTo(storageBlock, options);

            var actions =
                new[] { "login", "logout", "download", "upload", "delete" };

            foreach (var action in actions)
                parseBlock.Post(action);

            parseBlock.Complete();

            await storageBlock.Completion;
        }

        // TransformMany: one input produces multiple outputs
        public static async Task TransformManyDemo()
        {
            // split sentence into words
            var splitBlock = new TransformManyBlock<string, string>(sentence =>
            {
                Console.WriteLine($"Splitting: {sentence}");
                return sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            });

            // process each word
            var processWord = new TransformBlock<string, string>(word =>
            {
                Console.WriteLine($"Processing word: {word}");
                return word.ToUpper();
            });

            // final stage
            var printer = new ActionBlock<string>(word =>
            {
                Console.WriteLine($"Result word: {word}");
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            splitBlock.LinkTo(processWord, options);
            processWord.LinkTo(printer, options);

            splitBlock.Post("dataflow makes pipelines easy");
            splitBlock.Post("parallel processing is powerful");

            splitBlock.Complete();

            await printer.Completion;
        }

        // multiple workers send results to one collector
        public static async Task FanInMergeDemo()
        {
            var source = new BufferBlock<int>();

            var workerOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 2
            };

            // workers simulate heavy work
            var worker = new TransformBlock<int, int>(async x =>
            {
                Console.WriteLine($"Worker processing {x} on thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(300);
                return x * x;
            }, workerOptions);

            // collector gathers results
            var collector = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Collected result {x}");
            });

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            source.LinkTo(worker, options);
            worker.LinkTo(collector, options);

            for (int i = 1; i <= 8; i++)
                source.Post(i);

            source.Complete();

            await collector.Completion;
        }

        // custom block built from multiple blocks
        public static async Task CustomBlockEncapsulationDemo()
        {
            var customBlock = CreateCustomMathBlock();

            var printer = new ActionBlock<int>(x =>
            {
                Console.WriteLine($"Final result: {x}");
            });

            customBlock.LinkTo(printer, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });

            for (int i = 1; i <= 5; i++)
                customBlock.Post(i);

            customBlock.Complete();

            await printer.Completion;
        }

        // custom pipeline: multiply -> add -> divide
        private static IPropagatorBlock<int, int> CreateCustomMathBlock()
        {
            var multiply = new TransformBlock<int, int>(x => x * 2);
            var add = new TransformBlock<int, int>(x => x + 10);
            var divide = new TransformBlock<int, int>(x => x / 2);

            var options = new DataflowLinkOptions { PropagateCompletion = true };

            multiply.LinkTo(add, options);
            add.LinkTo(divide, options);

            // encapsulate internal pipeline
            return DataflowBlock.Encapsulate(multiply, divide);
        }

        // high-throughput message processing pipeline
        public static async Task HighThroughputMessagePipelineDemo()
        {
            var random = new Random();

            // st. 1: ingestion (buffer incoming messages)
            var inputBuffer = new BufferBlock<Message>(new DataflowBlockOptions
            {
                BoundedCapacity = 100 // backpressure
            });

            // st. 2: validation
            var validateBlock = new TransformManyBlock<Message, Message>(msg =>
            {
                if (msg.UserId % 7 == 0)
                {
                    Console.WriteLine($"Rejected message from user {msg.UserId}");
                    return Enumerable.Empty<Message>();
                }

                return new[] { msg };
            });

            // st. 3: processing (parallel CPU work)
            var processBlock = new TransformBlock<Message, ProcessedMessage>(
                msg =>
                {
                    var score = msg.UserId % 10;

                    return new ProcessedMessage
                    {
                        UserId = msg.UserId,
                        Content = msg.Content.ToUpper(),
                        RiskScore = score
                    };
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                });

            // st. 4: batching
            var batchBlock = new BatchBlock<ProcessedMessage>(10);

            // st. 5: persistence
            var storageBlock = new ActionBlock<ProcessedMessage[]>(batch =>
            {
                Console.WriteLine($"Persisting batch of {batch.Length} messages");

                foreach (var msg in batch)
                {
                    Console.WriteLine($"Stored message from user {msg.UserId}");
                }
            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            inputBuffer.LinkTo(validateBlock, linkOptions);
            validateBlock.LinkTo(processBlock, linkOptions);
            processBlock.LinkTo(batchBlock, linkOptions);
            batchBlock.LinkTo(storageBlock, linkOptions);

            // simulate message stream
            for (int i = 1; i <= 30; i++)
            {
                await inputBuffer.SendAsync(new Message
                {
                    UserId = random.Next(1, 50),
                    Content = $"message_{i}"
                });
            }

            inputBuffer.Complete();

            await storageBlock.Completion;
        }
    }
}
