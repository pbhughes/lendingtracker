using System;
using System.Collections.Generic;

namespace LendingTrackerApi.Models;

public partial class StandardMessage
{
    public int Id { get; set; }

    public string Text { get; set; } = null!;
}
