using System;
using System.Collections.Generic;

namespace TodoAPI.DbModels;

public partial class Todo
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int StatusId { get; set; }

    public DateTime DueDate { get; set; }

    public int UserId { get; set; }

    public virtual TodoStatus Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
