using System;
using System.Collections.Generic;

namespace TodoAPI.DbModels;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int TodoId { get; set; }

    public DateTimeOffset TimeStamp { get; set; }

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public virtual Todo Todo { get; set; } = null!;
}
