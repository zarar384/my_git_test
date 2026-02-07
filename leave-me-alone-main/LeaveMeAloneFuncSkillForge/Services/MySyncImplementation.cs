using LeaveMeAloneFuncSkillForge.Interfaces;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class MySyncImplementation : IMyAsyncInterface
    {
        private readonly Random _random = new Random();
        private readonly IFeatureFlagService _featureFlagService;

        public MySyncImplementation(IFeatureFlagService featureFlagService)
        {
            _featureFlagService = featureFlagService;
        }

        public Task DoSomethingAsync()
        {
            return Task.CompletedTask;
        }

        public Task DoSomethingWithExceptionAsync()
        {
            try
            {
                bool chance50 = _random.Next(2) == 0;
                
                // Simulate some synchronous work that throws an exception
                Task.Delay(100).Wait();

                if (chance50)
                    throw new InvalidOperationException("[EXCEPTION] Task failed due to some issue.");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        public async Task<string> GetPaymentMethodWithExceptionAsync(CancellationToken cancellationToken = default)
        {
            bool chance50 = _random.Next(2) == 0;

            // Simulate some synchronous work that throws an exception
            await Task.Delay(100, cancellationToken);

            if (chance50)
            {
                throw new InvalidOperationException("[EXCEPTION] Task failed due to some issue.");
            }

            // Check feature flag
            var paymentMethod = await GetPaymentMethodAsync(cancellationToken);

            if(string.IsNullOrEmpty(paymentMethod))
            {
                throw new Exception("[EXCEPTION] Payment method is not available.");
            }

            return paymentMethod;
        }

        public async Task<string> GetPaymentMethodAsync(CancellationToken cancellationToken = default)
        {
            var isNewEnabled = await _featureFlagService.IsNewPaymentMethodEnabledAsync(cancellationToken);

            return isNewEnabled
                ? "NewPaymentMethod"
                : "OldPaymentMethod";
        }

        public Task<int> GetIntAsync()
        {
            return Task.FromResult(_random.Next(1, 101));
        }

        public async Task<double> CalculatePriceAsync(Transaction transaction)
        {
            var isCheckoutNewEnabled = await _featureFlagService.IsNewCheckoutEnabledAsync();
            double basePrice = (double)transaction.Amount * _random.Next(1, 5);

            if (isCheckoutNewEnabled)
            {
                // apply a 10% discount for the new checkout feature
                return basePrice * 0.9;
            }
            else
            {
                return basePrice;
            }
        }

        public Task<T> NotImplementedAsync<T>()
        {
            return Task.FromException<T>(new NotImplementedException("[WRONG] This method is not implemented."));
        }

        public async Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction, 
            IProgress<double>? progress = null, 
            CancellationToken cancellationToken = default)
        {
            var featureFlagTask = _featureFlagService.IsNewCheckoutEnabledAsync(cancellationToken);

            return await CalculatePriceWithProgressAsync(transaction, progress, cancellationToken, featureFlagTask);
        }
       
        public ValueTask<double> CalculatePriceSmartWithProgressAsync(
            Transaction transaction, 
            IProgress<double>? progress = null, 
            CancellationToken cancellationToken = default)
        {
            // validation sync
            if (transaction == null)
                return ValueTask.FromException<double>(
                    new ArgumentNullException(nameof(transaction)));

            if (transaction.Amount < 0)
                return ValueTask.FromException<double>(
                    new ArgumentOutOfRangeException(nameof(transaction.Amount), "Amount cannot be negative."));

            progress?.Report(0.0);
            var checkoutFeatureFlagTask = _featureFlagService.IsNewCheckoutEnabledFastAsync(cancellationToken);

            // if the feature flag task is already completed, we can proceed synchronously
            if (checkoutFeatureFlagTask.IsCompletedSuccessfully)
                return new ValueTask<double>(CalculatePriceSyncWithProgress(transaction, checkoutFeatureFlagTask.Result, progress));

            // otherwise, we need to await the feature flag task asynchronously
            return new ValueTask<double>(CalculatePriceWithProgressAsync(transaction, progress, cancellationToken, checkoutFeatureFlagTask));
        }

        /// <summary>
        /// Demonstrates consuming ValueTask by converting it to Task
        /// in order to await it together with other asynchronous operations.
        /// </summary>
        public async Task<string> BuildCheckoutSummaryAsync(
            Transaction transaction,
            CancellationToken cancellationToken = default)
        {
            // get the feature flag as a ValueTask: may already be completed
            ValueTask<bool> checkoutFlagValueTask = _featureFlagService.IsNewCheckoutEnabledFastAsync(cancellationToken);

            // convert ValueTask to Task for easier composition with other async operations
            Task<bool> checkoutFlagTask = checkoutFlagValueTask.AsTask();

            // other independent async operations (I/O bound, simulated with Task.Delay here)
            Task<Film> filmTask = LoadFilmAsync(transaction.Id, cancellationToken); 
            Task<TaskData> taskDataTask = LoadTaskDataAsync(transaction.Id, cancellationToken);

            await Task.WhenAll(checkoutFlagTask, filmTask, taskDataTask);

            // now can safely access the results of all tasks.
            // here await is like result since we know they are completed, but using await allows for better exception handling and readability
            bool isCheckoutNewEnabled = await checkoutFlagTask;
            Film film = await filmTask;
            TaskData taskData = await taskDataTask;

            string checkoutType = isCheckoutNewEnabled ? "New Checkout" : "Old Checkout";

            return $"Checkout Summary:\n" +
                   $"* Transaction ID: {transaction.Id}\n" +
                   $"* Amount: {transaction.Amount:C}\n" +
                   $"* Time: {transaction.Time}\n" +
                   $"* Film: {film.Title} ({film.Genre})\n" +
                   $"* Box Office: {film.BoxOfficeRevenue:C}\n" +
                   $"* Task Complexity: {taskData.ComplexityLevel}\n" +
                   $"* Estimated Hours: {taskData.EstimatedHours}\n" +
                   $"* Urgency: {(taskData.IsUrgent ? "High" : "Normal")}\n" +
                   $"* Assigned Developer: {taskData.AssignedDeveloper}\n" +
                   $"* Backup Developer: {taskData.BackupDeveloper}\n" +
                   $"* Created Date: {taskData.CreatedDate}\n" +
                   $"* Due Date: {taskData.DueDate}\n" +
                   $"* Checkout Type: {checkoutType}";
        }

        #region private helpers for CalculatePriceSmartWithProgressAsync
        // synchronous fast-path
        private double CalculatePriceSyncWithProgress(
            Transaction transaction,
            bool isCheckoutNewEnabled,
            IProgress<double>? progress)
        {
            progress?.Report(0.25);

            double price = (double)transaction.Amount * _random.Next(1, 5);
            progress?.Report(0.75);

            if (isCheckoutNewEnabled)
                price *= 0.9;

            progress?.Report(1.0);
            return price;
        }

        // shared async body calculation logic
        private async Task<double> CalculatePriceBodyAsync(
            Transaction transaction,
            IProgress<double>? progress,
            CancellationToken cancellationToken,
            bool isCheckoutNewEnabled)
        {
            progress?.Report(0.25);
            await Task.Delay(300, cancellationToken);

            progress?.Report(0.5);
            await Task.Delay(300, cancellationToken);

            double price = (double)transaction.Amount * _random.Next(1, 5);
            progress?.Report(0.75);

            await Task.Delay(300, cancellationToken);

            if (isCheckoutNewEnabled)
                price *= 0.9;

            progress?.Report(1.0);
            return price;
        }

        // async wrapper for Task<bool> feature flag
        private async Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction,
            IProgress<double>? progress,
            CancellationToken cancellationToken,
            Task<bool> checkoutFeatureFlagTask)
        {
            bool isCheckoutNewEnabled = await checkoutFeatureFlagTask;
            return await CalculatePriceBodyAsync(
                transaction, progress, cancellationToken, isCheckoutNewEnabled);
        }

        // async wrapper for optimized ValueTask<bool> feature flag
        private async Task<double> CalculatePriceWithProgressAsync(
            Transaction transaction,
            IProgress<double>? progress,
            CancellationToken cancellationToken,
            ValueTask<bool> checkoutFeatureFlagTask)
        {
            bool isCheckoutNewEnabled = await checkoutFeatureFlagTask;

            return await CalculatePriceBodyAsync(transaction, progress, cancellationToken, isCheckoutNewEnabled);
        }
        #endregion

        #region private helper for BuildCheckoutSummaryAsync
        private async Task<Film> LoadFilmAsync(int filmId, CancellationToken cancellationToken)
        {
            // Simulate async loading of film data
            await Task.Delay(200, cancellationToken);

            // For demo purposes, return a fake film based on the ID
            return new Film
            {
                Id = filmId,
                Title = $"Film {filmId}",
                Genre = "Action",
                BoxOfficeRevenue = 100_000_000 * filmId
            };
        }

        private async Task<TaskData> LoadTaskDataAsync(int taskId, CancellationToken cancellationToken)
        {
            // Simulate async loading of task data
            await Task.Delay(200, cancellationToken);

            // For demo purposes, return fake task data based on the ID
            return new TaskData
            {
                EstimatedHours = _random.Next(4, 40),
                ComplexityLevel = _random.Next(1, 10),
                IsUrgent = _random.NextDouble() > 0.7,
                AssignedDeveloper = "Alice",
                BackupDeveloper = "Bob",
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(5)
            };
        }

        #endregion
    }
}
