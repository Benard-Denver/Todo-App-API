using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TodoAPI.DbModels;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class NotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("NotificationService started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TodoContext>();

                var now = DateTime.UtcNow;

                // 🔥 Get todos that need notifications
                var todos = await db.Todos
                    .Include(t => t.User)
                    .Where(t => t.NotificationTime <= now && t.Notify == true)
                    .ToListAsync();

                Console.WriteLine($"Found {todos.Count} todos to notify");

                foreach (var todo in todos)
                {
                    // 🔔 Insert into Notification table
                    var notification = new Notification
                    {
                        TodoID = todo.Id,
                        Todo = todo,
                        TimeStamp = DateTime.UtcNow,
                        Message = $"Reminder: {todo.Title} is due"
                    };

                    db.Notifications.Add(notification);

                    // prevent duplicate notifications
                    todo.Notify = false;

                    Console.WriteLine($"Notification created for Todo: {todo.Title}");
                }

                await db.SaveChangesAsync();

                // check every 30 seconds (good for testing)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}