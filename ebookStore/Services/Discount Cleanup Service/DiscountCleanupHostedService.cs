using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ebookStore.Services;

namespace ebookStore.BackgroundServices
{
    public class DiscountCleanupHostedService : IHostedService, IDisposable
    {
        private readonly DiscountCleanupService _discountCleanupService;
        private Timer _timer;

        public DiscountCleanupHostedService(DiscountCleanupService discountCleanupService)
        {
            _discountCleanupService = discountCleanupService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start the timer to run the cleanup task every hour
            _timer = new Timer(ExecuteCleanupTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async void ExecuteCleanupTask(object state)
        {
            try
            {
                // Execute the discount cleanup task
                await _discountCleanupService.RevertExpiredDiscountsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing discount cleanup task: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer when the service is stopped
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose of the timer when the service is disposed
            _timer?.Dispose();
        }
    }
}