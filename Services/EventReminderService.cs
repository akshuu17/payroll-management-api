using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Data;

namespace PayrollManagementAPI.Services
{
    public class EventReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventReminderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var tomorrow = DateTime.Today.AddDays(1);

                var events = await context.Events
                    .Where(e => e.EventDate.Date == tomorrow)
                    .ToListAsync();

                foreach (var ev in events)
                {
                    var registeredEmployees = await context.EventRegistrations
                        .Where(r => r.EventId == ev.Id)
                        .Join(context.Employees,
                            r => r.EPFNo,
                            e => e.EPFNo,
                            (r, e) => e)
                        .ToListAsync();

                    foreach (var emp in registeredEmployees)
                    {
                        if (!string.IsNullOrEmpty(emp.Email))
                        {
                            await emailService.SendEventEmail(
                                emp.Email,
                                $"Reminder: {ev.Title}",
                                ev.EventDate
                            );
                        }
                    }
                }

                // 🔥 Wait 1 Hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}