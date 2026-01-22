namespace LeaveMeAloneFuncSkillForge.Playground
{
    public static class DataSharingSynchronizationL
    {
        public static void Run()
        {
            ReaderWriterLockSlimDemo();
        }

        static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        static Random random = new Random();
        public static void ReaderWriterLockSlimDemo()
        {
            int x = 0;

            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    // Each writer will acquire write lock before modifying the balance
                    // Writers are exclusive, so only one writer can hold the lock at a time
                    rwLock.EnterWriteLock();
                    Console.WriteLine($"Writer {Task.CurrentId} acquired write lock, x = {x}.");
                    Thread.Sleep(3000); 

                    rwLock.ExitWriteLock();
                    Console.WriteLine($"Writer {Task.CurrentId} released write lock, x = {x}.");
                }));
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                ex.Handle(inner =>
                {
                    Console.WriteLine($"Exception: {inner.Message}");
                    return true;
                });
            }

            while(true)
            {
                Console.ReadKey();
                rwLock.EnterReadLock();
                Console.WriteLine($"Reader acquired read lock, x = {x}");

                int newValue = random.Next(10);
                x = newValue;
                Console.WriteLine($"Reader updated x to {x}");
                rwLock.ExitReadLock();
                Console.WriteLine($"Reader released read lock, x = {x}");
            }
        }

        // true means enable thread owner tracking for better debugging, but SpinLock is not reentrant,
        // so LockRecursionException will be thrown on second attempt to acquire the lock by the same thread
        static SpinLock spinLock = new SpinLock(true); // with false, LockRecursionException will not be thrown and deadlock will occur instead
        private static void LockRecursion(int x)
        {
            bool lockTaken = false;

            try
            {
                spinLock.Enter(ref lockTaken); // this will throw LockRecursionException on second call when x == 2, because SpinLock is not reentrant
            }
            catch (LockRecursionException ex)
            {
                Console.WriteLine($"LockRecursionException caught at level {x}: {ex.Message}");
                return;
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Lock acquired at level {x}");
                    LockRecursion(x - 1);// recursive call, will fail on second call, because SpinLock is not reentrant
                    spinLock.Exit();
                }
                else
                {
                    Console.WriteLine($"Lock not acquired at level {x}");
                }
            }
        }

        public static void AccountTransferWithMutexDemo()
        {
            var tasks = new List<Task>();
            var a = new Account(1000);
            var a2 = new Account(1000);

            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        // Using Mutex to control access to shared data
                        // WaitOne() blocks until the mutex is acquired
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            // account += amount
                            a.Deposit(1);
                        }
                        finally
                        {
                            if (haveLock)
                                mutex.ReleaseMutex();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            // account 2 -= amount
                            a2.Withdraw(1);
                        }
                        finally
                        {
                            if (haveLock)
                                mutex2.ReleaseMutex();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        // Transfer money between two accounts with two mutexes
                        // To avoid deadlock, always acquire the mutexes in the same order
                        bool haveLock = WaitHandle.WaitAll(new WaitHandle[] { mutex, mutex2 });
                        try
                        {
                            // account -= amount, account 2 += amount
                            a.Transfer(a2, 1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Final balance and balance2 should be 1000, because of controlling access to shared data with locks
            // balance: 1000 + (10 * 1000 * 1) - (10 * 1000 * 1) - (10 * 1000 * 1) + (10 * 1000 * 1) = 1000
            // balance2: 1000 - (10 * 1000 * 1) + (10 * 1000 * 1) + (10 * 1000 * 1) - (10 * 1000 * 1) = 1000
            Console.WriteLine($"Final Balance: {a.Balance}");
            Console.WriteLine($"Final Balance 2: {a2.Balance}");

        }

        public static void AccountSpinLockDemo()
        {
            var tasks = new List<Task>();
            var a = new AccountWithLock(1000);

            SpinLock spinLock = new SpinLock();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        // Using SpinLock to control access to shared data
                        // SpinLock is a value type, so need to use a local variable to track lock state
                        bool lockTaken = false;
                        try
                        {
                            spinLock.Enter(ref lockTaken);
                            a.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken)
                                spinLock.Exit();
                        }
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool lockTaken = false;
                        try
                        {
                            spinLock.Enter(ref lockTaken);
                            a.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken)
                                spinLock.Exit();
                        }
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Final balance should be 1000, because of controlling access to shared data with locks
            Console.WriteLine($"Final Balance: {a.Balance}");
        }

        public static void AccountLockDemo()
        {
            var tasks = new List<Task>();
            var a = new AccountWithLock(1000);

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        a.Deposit(100);
                    }
                }));

                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        a.Withdraw(100);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Final balance should be 1000, because of controlling access to shared data with locks
            Console.WriteLine($"Final Balance: {a.Balance}");
        }

        // Ensures that only a single instance of the application runs at a time.
        // If another instance is already running, the new instance will exit.
        // Also demonstrates multiple threads within the same process competing for the same named mutex.
        public static void MutexSingleInstanceAppDemo(string appName)
        {
            string processName = $"Process_{Guid.NewGuid().ToString().Substring(0, 4)}";

            Mutex processMutex;
            bool isOwner = false;

            try
            {
                processMutex = Mutex.OpenExisting(appName);
                Console.WriteLine($"{processName}: Another instance is already running. Exiting this instance.");
                return; // exit if another instance is running
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Console.WriteLine($"{processName}: No other instance is running. Continuing execution.");
                processMutex = new Mutex(false, appName);
            }

            // Create a mutex for threads within this process
            Mutex threadMutex = new Mutex();

            // Start 3 threads that will attempt to acquire the same named mutex
            for (int i = 0; i < 3; i++)
            {
                int threadId = i;
                Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Thread {threadId} is waiting to acquire the mutex...");
                    threadMutex.WaitOne();
                    Console.WriteLine($"Thread {threadId} has acquired the mutex.");
                    // Simulate some work
                    Thread.Sleep(2000);
                    Console.WriteLine($"Thread {threadId} is releasing the mutex.");
                    threadMutex.ReleaseMutex();
                });
            }

            Console.WriteLine("Press any key to exit and release the process mutex...");
            Console.ReadKey();

            // Release the system-wide mutex so other process instances can run
            processMutex.ReleaseMutex();
        }

        private class AccountWithLock
        {
            public object _lock = new object();
            public int Balance { get; private set; }

            public AccountWithLock(int initialBalance)
            {
                Balance = initialBalance;
            }

            public void Deposit(int amount)
            {
                // +=
                // operation is not atomic
                // operation 1: temp = get_Balance() + amount
                // operation 2: set_Balance(temp)
                // if two threads execute this at the same time

                lock (_lock)
                {
                    Balance += amount;
                }
            }

            public void Withdraw(int amount)
            {
                lock (_lock)
                {
                    Balance -= amount;
                }
            }
        }

        private class AccountWithInterlocked
        {
            private int balance;

            public int Balance
            {
                get { return balance; }
                private set { balance = value; }
            }

            public AccountWithInterlocked(int initialBalance)
            {
                Balance = initialBalance;
            }

            public void Deposit(int amount)
            {
                // +=
                // operation 1: temp = get_Balance() + amount
                // operation 2: set_Balance(temp) 
                Interlocked.Add(ref balance, amount);
            }

            public void Withdraw(int amount)
            {
                Interlocked.Add(ref balance, -amount);
            }
        }

        private class Account
        {
            private int balance;

            public int Balance
            {
                get { return balance; }
                private set { balance = value; }
            }

            public Account(int initialBalance)
            {
                Balance = initialBalance;
            }

            public void Deposit(int amount)
            {
                // +=
                // operation 1: temp = get_Balance() + amount
                // operation 2: set_Balance(temp) 
                balance += amount;
            }

            public void Withdraw(int amount)
            {
                balance -= amount;
            }

            public void Transfer(Account to, int amount)
            {
                this.Withdraw(amount);
                to.Deposit(amount);
            }
        }
    }
}
