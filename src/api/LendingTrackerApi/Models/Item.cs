using System;
using System.Collections.Generic;

namespace LendingTrackerApi.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public Guid OwnerId { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User Owner { get; set; } = null!;

    public virtual Transaction Transaction { get; set; }
}
