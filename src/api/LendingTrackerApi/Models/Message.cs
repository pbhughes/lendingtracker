using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LendingTrackerApi.Models;

public partial class Message
{
    public Guid Id { get; set; } = new Guid();

    public string Method { get; set; } = null!;

    public int Direction { get; set; }

    public string Text { get; set; } = null!;

    public DateTime MessageDate { get; set; }

    public string Phone { get; set; } = null!;

    public int? ItemId { get; set; }

    public Guid TransactionId { get; set; }

    [JsonIgnore]
    public virtual Item? Item { get; set; }

    [JsonIgnore]
    public virtual Transaction Transaction { get; set; } = null!;
}
