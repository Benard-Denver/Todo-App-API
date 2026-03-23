using System;
using System.Collections.Generic;

namespace TodoAPI.DbModels;

public partial class TodoStatus
{
    public int StatusId { get; set; }

    public string StatusOption { get; set; } = null!;

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
