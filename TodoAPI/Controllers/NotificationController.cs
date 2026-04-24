using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.DbModels;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly TodoContext _context;

    public NotificationController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        var notifications = await _context.Notifications
            .Include(n => n.Todo)
            .Where(n => n.Todo.UserId == userId)
            .OrderByDescending(n => n.TimeStamp)
            .ToListAsync();

        return Ok(notifications);
    }

    [HttpPut("read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);

        if (notification == null)
            return NotFound();

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok();
    }
}