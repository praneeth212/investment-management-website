using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Management.Services
{
    // Implementing IHostedService to run scheduled tasks in the background
    public class ScheduledTasks : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        // Constructor that takes IServiceProvider to resolve dependencies
        public ScheduledTasks(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Starts the scheduled task when the service is started.
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Schedule the task to run every 24 hrs
            _timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromHours(12));
            return Task.CompletedTask;
        }

        // Executes the scheduled task.
        private async void ExecuteTask(object state)
        {
            // Creating a scope for the service provider to resolve scoped services
            using (var scope = _serviceProvider.CreateScope())
            {
                // Resolving the SipReminderScheduler service from the service provider
                var scheduler = scope.ServiceProvider.GetRequiredService<SipReminderScheduler>();
                // Calling the method in 'SipReminderScheduler' service
                await scheduler.CheckAndSendRemindersAsync();
            }
        }

        // Stops the scheduled task when the service is stopped.
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stopping the timer by changing its due time to infinite (it will no longer trigger)
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        // Dispose method to clean up resources when the service is disposed
        public void Dispose()
        {
            // Dispose the timer to release resources
            _timer?.Dispose();
        }
    }
}