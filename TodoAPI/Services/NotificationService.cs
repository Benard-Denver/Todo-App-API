using Microsoft.EntityFrameworkCore;
using TodoAPI.DbModels;

namespace TodoAPI.Services
{
    public class TodoNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TodoNotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TodoContext>();

                var now = DateTime.UtcNow;

                var dueTodos = await db.Todos
                    .Where(t =>
                        t.Notify == true &&
                        t.NotificationTime != null &&
                        t.NotificationTime <= now)
                    .ToListAsync(stoppingToken);

                foreach (var todo in dueTodos)
                {
                    // prevent duplicate notifications
                    var alreadyNotified = await db.Notifications
                        .AnyAsync(n => n.TodoId == todo.Id && n.Message.Contains("Reminder"), stoppingToken);

                    if (alreadyNotified)
                        continue;

                    db.Notifications.Add(new Notification
                    {
                        TodoId = todo.Id,
                        Message = $"Reminder: '{todo.Title}' is due",
                        TimeStamp = DateTimeOffset.UtcNow,
                        IsRead = false
                    });
                }

                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(60000, stoppingToken); // every 1 minute
            }
        }
    }
}