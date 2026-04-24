using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.DbModels;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class NotificationController : ControllerBase
    {
        private readonly TodoContext dB;

        public NotificationController(TodoContext context)
        {
            dB = context;
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var notifications = await dB.Notifications
                .Include(n => n.Todo)
                .ThenInclude(t => t.User)
                .Where(n => n.Todo.User.Username == username && !n.isRead)
                .OrderByDescending(n => n.TimeStamp)
                .ToListAsync();

            return Ok(notifications);
        }
    }
}